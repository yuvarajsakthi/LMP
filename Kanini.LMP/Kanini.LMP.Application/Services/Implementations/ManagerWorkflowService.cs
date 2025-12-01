using AutoMapper;
using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using Kanini.LMP.Database.Entities.ManagerEntities;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.AppliedLoans;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.ManagerDashboardDto.Manager.NewFolderBasicDto;
using Kanini.LMP.Database.Enums;
using Microsoft.Extensions.Logging;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class ManagerWorkflowService : IManagerWorkflowService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IRazorpayService _razorpayService;
        private readonly IMapper _mapper;
        private readonly ILogger<ManagerWorkflowService> _logger;

        public ManagerWorkflowService(
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            IRazorpayService razorpayService,
            IMapper mapper,
            ILogger<ManagerWorkflowService> logger)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _razorpayService = razorpayService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<AppliedLoanListDto>> GetPendingApplicationsAsync()
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingPendingApplications);

                var applications = await _unitOfWork.LoanApplications.GetAllAsync();
                var customers = await _unitOfWork.Customers.GetAllAsync();
                var users = await _unitOfWork.Users.GetAllAsync();

                var result = applications
                    .Where(app => app.Status == ApplicationStatus.Submitted || app.Status == ApplicationStatus.Pending)
                    .Select(app =>
                    {
                        var applicant = app?.Applicants?.FirstOrDefault();
                        var customer = applicant != null ? customers.FirstOrDefault(c => c.CustomerId == applicant.CustomerId) : null;
                        var user = users.FirstOrDefault(u => u.UserId == customer?.UserId);

                        return new AppliedLoanListDto
                        {
                            ApplicationId = app.LoanApplicationBaseId,
                            CustomerId = customer?.CustomerId ?? 0,
                            CustomerFullName = user?.FullName ?? ApplicationConstants.Messages.Unknown,
                            LoanProductType = app.LoanProductType,
                            ApplicationNumber = $"LMP{app.LoanApplicationBaseId:D6}",
                            RequestedLoanAmount = 0, // Should come from LoanDetails repository
                            SubmissionDate = app.SubmissionDate.ToDateTime(TimeOnly.MinValue),
                            Status = app.Status
                        };
                    });

                _logger.LogInformation(ApplicationConstants.Messages.PendingApplicationsCompleted);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.PendingApplicationsFailed);
                throw new Exception(ApplicationConstants.ErrorMessages.PendingApplicationsFailed);
            }
        }

        public async Task<LoanApprovalDetailDto> GetApplicationDetailsAsync(int applicationId)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingApplicationDetails, applicationId);

                var application = await _unitOfWork.LoanApplications.GetByIdAsync(applicationId);
                var workflows = await _unitOfWork.LoanWorkflows.GetAllAsync();
                var customers = await _unitOfWork.Customers.GetAllAsync();
                var users = await _unitOfWork.Users.GetAllAsync();
                // Note: LoanDetails should have its own repository, using workaround for now
                var loanDetails = new List<LoanDetails>(); // Placeholder - needs proper LoanDetails repository

                var customer = customers.FirstOrDefault(c => c.CustomerId == application?.Applicants?.FirstOrDefault()?.CustomerId);
                var user = customer != null ? users.FirstOrDefault(u => u.UserId == customer.UserId) : null;
                var loanDetail = loanDetails.FirstOrDefault(ld => ld.LoanApplicationBaseId == applicationId);
                var appWorkflows = workflows.Where(w => w.LoanApplicationBaseId == applicationId);

                var result = new LoanApprovalDetailDto
                {
                    LoanApplicationId = applicationId,
                    ApplicationNumber = $"LMP{applicationId:D6}",
                    LoanProductType = application?.LoanProductType ?? "PersonalLoan",
                    CurrentStatus = application?.Status ?? ApplicationStatus.Pending,
                    CustomerFullName = user?.FullName ?? ApplicationConstants.Messages.Unknown,
                    CustomerId = customer?.CustomerId.ToString() ?? "0",
                    CustomerOccupation = customer?.Occupation ?? ApplicationConstants.Messages.Unknown,
                    ProfileImageBase64 = Convert.ToBase64String(customer?.ProfileImage ?? new byte[0]),
                    RequestedLoanAmount = loanDetail?.RequestedAmount ?? 0,
                    RequestedTenureMonths = loanDetail?.TenureMonths ?? 0,
                    EligibilityAmountCalculated = loanDetail?.RequestedAmount ?? 0,
                    EligibilityMessage = "Eligible based on credit score and income",
                    RevisedAmountOffered = loanDetail?.RequestedAmount ?? 0,
                    RevisedTenureMonthsOffered = loanDetail?.TenureMonths ?? 0,
                    SignatureImageBase64 = "",
                    IDProofImageBase64 = "",
                    OriginationPipeline = appWorkflows.Select(w => new WorkflowStepDto
                    {
                        StepName = w.StepName.ToString(),
                        CompletionDate = w.CompletionDate,
                        IsCompleted = w.StepStatus == StepStatus.Completed,
                        StatusIndicator = w.StepStatus.ToString()
                    }).ToList(),
                    KYCEligibilityScore = customer?.CreditScore ?? 0,
                    LoanDefaultScore = 85.0m,
                    OverallEligibilityScore = customer?.CreditScore ?? 0
                };

                _logger.LogInformation(ApplicationConstants.Messages.ApplicationDetailsCompleted, applicationId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.ApplicationDetailsFailed, applicationId);
                throw new Exception(ApplicationConstants.ErrorMessages.ApplicationDetailsFailed);
            }
        }

        public async Task<LoanOriginationWorkflowDTO> StartWorkflowAsync(int applicationId, int managerId)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingWorkflowStart, applicationId, managerId);

                using var transaction = await _unitOfWork.BeginTransactionAsync();

                var workflow = new LoanOriginationWorkflow
                {
                    LoanApplicationBaseId = applicationId,
                    StepName = ManagerEnum.Review,
                    StepStatus = StepStatus.InProgress,
                    ManagerId = managerId,
                    ManagerNotes = "Workflow started - Initial review in progress"
                };

                var created = await _unitOfWork.LoanWorkflows.AddAsync(workflow);

                // Update application status
                var application = await _unitOfWork.LoanApplications.GetByIdAsync(applicationId);
                if (application != null)
                {
                    application.Status = ApplicationStatus.Pending;
                    await _unitOfWork.LoanApplications.UpdateAsync(application);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation(ApplicationConstants.Messages.WorkflowStartCompleted, applicationId);
                return MapToWorkflowDto(created);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.WorkflowStartFailed, applicationId);
                throw new Exception(ApplicationConstants.ErrorMessages.WorkflowStartFailed);
            }
        }

        public async Task<LoanOriginationWorkflowDTO> UpdateWorkflowStepAsync(int applicationId, ManagerEnum stepName, StepStatus status, string notes, int managerId)
        {
            var workflows = await _unitOfWork.LoanWorkflows.GetAllAsync();
            var workflow = workflows.FirstOrDefault(w => w.LoanApplicationBaseId == applicationId && w.StepName == stepName);

            if (workflow == null)
            {
                workflow = new LoanOriginationWorkflow
                {
                    LoanApplicationBaseId = applicationId,
                    StepName = stepName,
                    StepStatus = status,
                    ManagerId = managerId,
                    ManagerNotes = notes,
                    CompletionDate = status == StepStatus.Completed ? DateTime.UtcNow : null
                };
                workflow = await _unitOfWork.LoanWorkflows.AddAsync(workflow);
            }
            else
            {
                workflow.StepStatus = status;
                workflow.ManagerNotes = notes;
                workflow.ManagerId = managerId;
                workflow.CompletionDate = status == StepStatus.Completed ? DateTime.UtcNow : workflow.CompletionDate;
                workflow = await _unitOfWork.LoanWorkflows.UpdateAsync(workflow);
            }

            return MapToWorkflowDto(workflow);
        }

        public async Task<IEnumerable<WorkflowStepDto>> GetWorkflowStatusAsync(int applicationId)
        {
            var workflows = await _unitOfWork.LoanWorkflows.GetAllAsync();
            var appWorkflows = workflows.Where(w => w.LoanApplicationBaseId == applicationId);

            return appWorkflows.Select(w => new WorkflowStepDto
            {
                StepName = w.StepName.ToString(),
                CompletionDate = w.CompletionDate,
                IsCompleted = w.StepStatus == StepStatus.Completed,
                StatusIndicator = w.StepStatus.ToString()
            });
        }

        public async Task<bool> VerifyDocumentsAsync(int applicationId, int managerId, string verificationNotes)
        {
            await UpdateWorkflowStepAsync(applicationId, ManagerEnum.DocumentVerificationI, StepStatus.Completed, verificationNotes, managerId);
            await UpdateWorkflowStepAsync(applicationId, ManagerEnum.DocumentVerificationII, StepStatus.InProgress, "Starting detailed verification", managerId);
            return true;
        }

        public async Task<bool> ApproveApplicationAsync(int applicationId, int managerId, string approvalNotes)
        {
            await UpdateWorkflowStepAsync(applicationId, ManagerEnum.Decision, StepStatus.Completed, approvalNotes, managerId);

            var application = await _unitOfWork.LoanApplications.GetByIdAsync(applicationId);
            if (application != null)
            {
                application.Status = ApplicationStatus.Approved;
                application.ApprovedDate = DateOnly.FromDateTime(DateTime.UtcNow);
                await _unitOfWork.LoanApplications.UpdateAsync(application);
            }

            // Get customer for notification
            var applicant = application?.Applicants?.FirstOrDefault();
            var customer = applicant?.Customer;

            if (customer != null)
            {
                await _notificationService.NotifyLoanApprovedAsync(
                    customer.UserId,
                    managerId,
                    0,
                    applicationId);
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RejectApplicationAsync(int applicationId, int managerId, string rejectionReason)
        {
            await UpdateWorkflowStepAsync(applicationId, ManagerEnum.Decision, StepStatus.Failed, rejectionReason, managerId);

            var application = await _unitOfWork.LoanApplications.GetByIdAsync(applicationId);
            if (application != null)
            {
                application.Status = ApplicationStatus.Rejected;
                await _unitOfWork.LoanApplications.UpdateAsync(application);
            }

            // Notify customer of rejection
            var applicant = application?.Applicants?.FirstOrDefault();
            var customer = applicant?.Customer;
            if (customer != null)
            {
                await _notificationService.NotifyLoanRejectedAsync(
                    customer.UserId,
                    managerId,
                    rejectionReason,
                    applicationId);
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReviseApplicationAsync(int applicationId, LoanRevisionInputDto revisionData, int managerId)
        {
            // Note: LoanDetails repository not available in UnitOfWork, using placeholder
            // This would need to be implemented when LoanDetails repository is added to UnitOfWork

            await UpdateWorkflowStepAsync(applicationId, ManagerEnum.Decision, StepStatus.Completed, $"Revised: Amount={revisionData.NewApprovedAmount}, Tenure={revisionData.NewTenureMonths}", managerId);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DisburseApplicationAsync(int applicationId, int managerId, decimal disbursedAmount)
        {
            try
            {
                await UpdateWorkflowStepAsync(applicationId, ManagerEnum.Disbursed, StepStatus.InProgress, $"Disbursement initiated: {disbursedAmount}", managerId);

                // Get application and customer details
                var application = await _unitOfWork.LoanApplications.GetByIdAsync(applicationId);
                var applicant = application?.Applicants?.FirstOrDefault();
                var applicantCustomer = applicant?.Customer;
                var user = applicantCustomer != null ? await _unitOfWork.Users.GetByIdAsync(applicantCustomer.UserId) : null;
                var customerName = user?.FullName ?? "Customer";

                // Use Razorpay to transfer money to customer
                var razorpayTransactionId = await _razorpayService.TransferToCustomerAsync(applicationId, disbursedAmount, customerName);

                if (!string.IsNullOrEmpty(razorpayTransactionId))
                {
                    // Update application status
                    if (application != null)
                    {
                        application.Status = ApplicationStatus.Disbursed;
                        await _unitOfWork.LoanApplications.UpdateAsync(application);
                    }

                    // Create loan account with Razorpay transaction ID
                    if (applicantCustomer != null)
                    {
                        var loanAccount = new LoanAccount
                        {
                            LoanApplicationBaseId = applicationId,
                            CustomerId = applicantCustomer.CustomerId,
                            CurrentPaymentStatus = LoanPaymentStatus.Active,
                            DisbursementDate = DateTime.UtcNow,
                            TotalLoanAmount = disbursedAmount,
                            PrincipalRemaining = disbursedAmount,
                            LastStatusUpdate = DateTime.UtcNow,
                            // DisbursementTransactionId = razorpayTransactionId
                        };
                        await _unitOfWork.LoanAccounts.AddAsync(loanAccount);
                    }

                    await _unitOfWork.SaveChangesAsync();
                    await UpdateWorkflowStepAsync(applicationId, ManagerEnum.Disbursed, StepStatus.Completed, $"Disbursed ₹{disbursedAmount} via Razorpay: {razorpayTransactionId}", managerId);

                    // Notify customer
                    if (applicantCustomer != null)
                    {
                        await _notificationService.NotifyLoanDisbursedAsync(applicantCustomer.UserId, managerId, disbursedAmount, applicationId);
                    }

                    return true;
                }
                else
                {
                    await UpdateWorkflowStepAsync(applicationId, ManagerEnum.Disbursed, StepStatus.Failed, "Razorpay transfer failed", managerId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                await UpdateWorkflowStepAsync(applicationId, ManagerEnum.Disbursed, StepStatus.Failed, $"Disbursement failed: {ex.Message}", managerId);
                return false;
            }
        }

        private LoanOriginationWorkflowDTO MapToWorkflowDto(LoanOriginationWorkflow workflow)
        {
            return _mapper.Map<LoanOriginationWorkflowDTO>(workflow);
        }
    }
}