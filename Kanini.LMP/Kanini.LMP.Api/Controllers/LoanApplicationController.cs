using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.PersonalLoanApplication;
using Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.HomeLoanApplication;
using Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.VehicleLoanApplication;
using Kanini.LMP.Database.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Kanini.LMP.Api.Controllers
{
    [Route(ApplicationConstants.Routes.LoanApplicationController)]
    [ApiController]
    [Authorize]
    public class LoanApplicationController : ControllerBase
    {
        private readonly ILoanApplicationService _loanApplicationService;
        private readonly ILogger<LoanApplicationController> _logger;

        public LoanApplicationController(ILoanApplicationService loanApplicationService, ILogger<LoanApplicationController> logger)
        {
            _loanApplicationService = loanApplicationService;
            _logger = logger;
        }

        [HttpGet(ApplicationConstants.Routes.Personal)]
        public async Task<ActionResult<IReadOnlyList<PersonalLoanApplicationDTO>>> GetAllPersonalLoans()
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingLoanRetrieval, "all personal loans");

                var loans = await _loanApplicationService.GetAllPersonalLoansAsync();

                _logger.LogInformation(ApplicationConstants.Messages.LoanRetrievalCompleted, loans.Count);
                return Ok(loans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.LoanRetrievalFailed, "all personal loans");
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.LoanRetrievalFailed });
            }
        }

        [HttpGet(ApplicationConstants.Routes.PersonalById)]
        public async Task<ActionResult<PersonalLoanApplicationDTO>> GetPersonalLoan(int id)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingLoanRetrieval, id);

                var loan = await _loanApplicationService.GetPersonalLoanByIdAsync(id);
                if (loan == null)
                {
                    _logger.LogWarning(ApplicationConstants.ErrorMessages.LoanApplicationNotFound, id);
                    return NotFound(new { message = ApplicationConstants.ErrorMessages.LoanApplicationNotFound });
                }

                _logger.LogInformation(ApplicationConstants.Messages.LoanRetrievalCompleted, id);
                return Ok(loan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.LoanRetrievalFailed, id);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.LoanRetrievalFailed });
            }
        }

        [HttpGet(ApplicationConstants.Routes.PersonalByStatus)]
        public async Task<ActionResult<IReadOnlyList<PersonalLoanApplicationDTO>>> GetLoansByStatus(ApplicationStatus status)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingLoanRetrieval, $"loans with status {status}");

                var loans = await _loanApplicationService.GetLoansByStatusAsync(status);

                _logger.LogInformation(ApplicationConstants.Messages.LoanRetrievalCompleted, loans.Count);
                return Ok(loans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.LoanRetrievalFailed, $"status {status}");
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.LoanRetrievalFailed });
            }
        }

        [HttpPost(ApplicationConstants.Routes.PersonalSubmit)]
        public async Task<ActionResult> SubmitCompletePersonalLoan(int customerId, PersonalLoanApplicationCreateDTO dto)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingPersonalLoanCreation, customerId);

                var created = await _loanApplicationService.CreatePersonalLoanAsync(dto, customerId);
                var submitted = await _loanApplicationService.UpdateLoanStatusAsync(created.LoanApplicationBaseId, ApplicationStatus.Submitted);

                _logger.LogInformation(ApplicationConstants.Messages.PersonalLoanCreationCompleted, submitted.LoanApplicationBaseId);
                return Ok(new
                {
                    ApplicationId = submitted.LoanApplicationBaseId,
                    Status = submitted.Status,
                    Message = ApplicationConstants.Messages.LoanApplicationSubmittedSuccessfully,
                    NextSteps = new[]
                    {
                        "Upload required documents using /api/Document/upload endpoint",
                        "Wait for verification and approval",
                        "Check application status regularly"
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, ApplicationConstants.ErrorMessages.IneligibleForLoan);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.IneligibleForLoan });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.PersonalLoanCreationFailed, customerId);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.PersonalLoanCreationFailed });
            }
        }

        [HttpPut(ApplicationConstants.Routes.PersonalUpdateStatus)]
        public async Task<ActionResult<PersonalLoanApplicationDTO>> UpdateLoanStatus(int id, [FromBody] ApplicationStatus status)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingLoanStatusUpdate, id, status);

                var updated = await _loanApplicationService.UpdateLoanStatusAsync(id, status);

                _logger.LogInformation(ApplicationConstants.Messages.LoanStatusUpdateCompleted, id);
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, ApplicationConstants.ErrorMessages.LoanApplicationNotFound, id);
                return NotFound(new { message = ApplicationConstants.ErrorMessages.LoanApplicationNotFound });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.LoanStatusUpdateFailed, id);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.LoanStatusUpdateFailed });
            }
        }

        [HttpGet("customer-applications")]
        public async Task<ActionResult<IReadOnlyList<PersonalLoanApplicationDTO>>> GetCustomerApplications()
        {
            try
            {
                _logger.LogInformation("Processing customer applications retrieval");

                var loans = await _loanApplicationService.GetAllPersonalLoansAsync();

                _logger.LogInformation("Customer applications retrieval completed, found {0} applications", loans.Count);
                return Ok(loans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve customer applications");
                return BadRequest(new { message = "Failed to retrieve customer applications" });
            }
        }

        // Home Loan Endpoints
        [HttpPost(ApplicationConstants.Routes.Home)]
        public async Task<ActionResult<HomeLoanApplicationDTO>> CreateHomeLoan(int customerId, HomeLoanApplicationCreateDTO dto)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingHomeLoanCreation, customerId);

                var created = await _loanApplicationService.CreateHomeLoanAsync(dto, customerId);

                _logger.LogInformation(ApplicationConstants.Messages.HomeLoanCreationCompleted, created.LoanApplicationBaseId);
                return Ok(created);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, ApplicationConstants.ErrorMessages.IneligibleForLoan);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.IneligibleForLoan });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.HomeLoanCreationFailed, customerId);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.HomeLoanCreationFailed });
            }
        }

        // Vehicle Loan Endpoints
        [HttpPost(ApplicationConstants.Routes.Vehicle)]
        public async Task<ActionResult<VehicleLoanApplicationDTO>> CreateVehicleLoan(int customerId, VehicleLoanApplicationCreateDTO dto)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingVehicleLoanCreation, customerId);

                var created = await _loanApplicationService.CreateVehicleLoanAsync(dto, customerId);

                _logger.LogInformation(ApplicationConstants.Messages.VehicleLoanCreationCompleted, created.LoanApplicationBaseId);
                return Ok(created);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, ApplicationConstants.ErrorMessages.IneligibleForLoan);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.IneligibleForLoan });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.VehicleLoanCreationFailed, customerId);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.VehicleLoanCreationFailed });
            }
        }
    }
}