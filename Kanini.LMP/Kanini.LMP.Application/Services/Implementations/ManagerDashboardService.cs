using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using Kanini.LMP.Database.EntitiesDtos;
using Kanini.LMP.Database.Enums;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class ManagerDashboardService : IManagerDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ManagerDashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // 1. Dashboard Stats with Graphs
        public async Task<DashboardStatsDTO> GetDashboardStatsAsync()
        {
            var personalLoans = await _unitOfWork.PersonalLoanApplications.GetAllAsync();
            var homeLoans = await _unitOfWork.HomeLoanApplications.GetAllAsync();
            var vehicleLoans = await _unitOfWork.VehicleLoanApplications.GetAllAsync();
            var allApplications = personalLoans.Cast<LoanApplicationBase>().Concat(homeLoans).Concat(vehicleLoans).ToList();
            var emiPlans = await _unitOfWork.EMIPlans.GetAllAsync();

            var loanTypeDistribution = allApplications
                .GroupBy(a => a.LoanProductType)
                .Select(g => new LoanTypeDistributionDTO
                {
                    LoanType = g.Key.ToString(),
                    Count = g.Count(),
                    TotalAmount = g.Sum(a => a.RequestedLoanAmount)
                }).ToList();

            var monthlyTrend = allApplications
                .Where(a => a.SubmissionDate >= DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-6)))
                .GroupBy(a => new { a.SubmissionDate.Year, a.SubmissionDate.Month })
                .Select(g => new MonthlyApplicationTrendDTO
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    ApplicationCount = g.Count()
                }).ToList();

            return new DashboardStatsDTO
            {
                TotalApplications = allApplications.Count,
                PendingApplications = allApplications.Count(a => a.Status == ApplicationStatus.Pending || a.Status == ApplicationStatus.Submitted),
                ApprovedApplications = allApplications.Count(a => a.Status == ApplicationStatus.Approved),
                RejectedApplications = allApplications.Count(a => a.Status == ApplicationStatus.Rejected),
                DisbursedApplications = allApplications.Count(a => a.Status == ApplicationStatus.Disbursed),
                TotalDisbursedAmount = allApplications.Where(a => a.Status == ApplicationStatus.Disbursed).Sum(a => a.RequestedLoanAmount),
                ActiveLoans = emiPlans.Count(e => e.Status == EMIPlanStatus.Active),
                LoanTypeDistribution = loanTypeDistribution,
                MonthlyTrend = monthlyTrend
            };
        }

        // 2. Applied Loans
        public async Task<List<LoanApplicationDetailDTO>> GetAllLoanApplicationsAsync()
        {
            var personalLoans = await _unitOfWork.PersonalLoanApplications.GetAllAsync();
            var homeLoans = await _unitOfWork.HomeLoanApplications.GetAllAsync();
            var vehicleLoans = await _unitOfWork.VehicleLoanApplications.GetAllAsync();

            var result = new List<LoanApplicationDetailDTO>();
            foreach (var app in personalLoans.Cast<LoanApplicationBase>().Concat(homeLoans).Concat(vehicleLoans))
            {
                result.Add(await MapToDetailDTO(app));
            }
            return result;
        }

        public async Task<LoanApplicationDetailDTO?> GetLoanApplicationByIdAsync(int id)
        {
            var personalLoan = await _unitOfWork.PersonalLoanApplications.GetByIdAsync(id);
            if (personalLoan != null) return await MapToDetailDTO(personalLoan);

            var homeLoan = await _unitOfWork.HomeLoanApplications.GetByIdAsync(id);
            if (homeLoan != null) return await MapToDetailDTO(homeLoan);

            var vehicleLoan = await _unitOfWork.VehicleLoanApplications.GetByIdAsync(id);
            if (vehicleLoan != null) return await MapToDetailDTO(vehicleLoan);

            return null;
        }

        public async Task<List<LoanApplicationDetailDTO>> GetLoanApplicationsByStatusAsync(string status)
        {
            if (!Enum.TryParse<ApplicationStatus>(status, true, out var appStatus))
                return new List<LoanApplicationDetailDTO>();

            var personalLoans = (await _unitOfWork.PersonalLoanApplications.GetAllAsync()).Where(a => a.Status == appStatus);
            var homeLoans = (await _unitOfWork.HomeLoanApplications.GetAllAsync()).Where(a => a.Status == appStatus);
            var vehicleLoans = (await _unitOfWork.VehicleLoanApplications.GetAllAsync()).Where(a => a.Status == appStatus);

            var result = new List<LoanApplicationDetailDTO>();
            foreach (var app in personalLoans.Cast<LoanApplicationBase>().Concat(homeLoans).Concat(vehicleLoans))
            {
                result.Add(await MapToDetailDTO(app));
            }
            return result;
        }

        public async Task<bool> UpdateApplicationStatusAsync(UpdateApplicationStatusDTO dto)
        {
            if (!Enum.TryParse<ApplicationStatus>(dto.Status, true, out var newStatus))
                return false;

            var personalLoan = await _unitOfWork.PersonalLoanApplications.GetByIdAsync(dto.LoanApplicationBaseId);
            if (personalLoan != null)
            {
                personalLoan.Status = newStatus;
                if (dto.InterestRate.HasValue) personalLoan.InterestRate = dto.InterestRate.Value;
                if (!string.IsNullOrEmpty(dto.RejectionReason)) personalLoan.RejectionReason = dto.RejectionReason;
                if (newStatus == ApplicationStatus.Approved) personalLoan.ApprovedDate = DateOnly.FromDateTime(DateTime.UtcNow);
                await _unitOfWork.PersonalLoanApplications.UpdateAsync(personalLoan);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }

            var homeLoan = await _unitOfWork.HomeLoanApplications.GetByIdAsync(dto.LoanApplicationBaseId);
            if (homeLoan != null)
            {
                homeLoan.Status = newStatus;
                if (dto.InterestRate.HasValue) homeLoan.InterestRate = dto.InterestRate.Value;
                if (!string.IsNullOrEmpty(dto.RejectionReason)) homeLoan.RejectionReason = dto.RejectionReason;
                if (newStatus == ApplicationStatus.Approved) homeLoan.ApprovedDate = DateOnly.FromDateTime(DateTime.UtcNow);
                await _unitOfWork.HomeLoanApplications.UpdateAsync(homeLoan);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }

            var vehicleLoan = await _unitOfWork.VehicleLoanApplications.GetByIdAsync(dto.LoanApplicationBaseId);
            if (vehicleLoan != null)
            {
                vehicleLoan.Status = newStatus;
                if (dto.InterestRate.HasValue) vehicleLoan.InterestRate = dto.InterestRate.Value;
                if (!string.IsNullOrEmpty(dto.RejectionReason)) vehicleLoan.RejectionReason = dto.RejectionReason;
                if (newStatus == ApplicationStatus.Approved) vehicleLoan.ApprovedDate = DateOnly.FromDateTime(DateTime.UtcNow);
                await _unitOfWork.VehicleLoanApplications.UpdateAsync(vehicleLoan);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }

            return false;
        }

        // Helper Methods
        private async Task<LoanApplicationDetailDTO> MapToDetailDTO(LoanApplicationBase app)
        {
            var emiPlans = (await _unitOfWork.EMIPlans.GetAllAsync()).Where(e => e.LoanApplicationBaseId == app.LoanApplicationBaseId);
            var emiPlan = emiPlans.FirstOrDefault();

            return new LoanApplicationDetailDTO
            {
                LoanApplicationBaseId = app.LoanApplicationBaseId,
                CustomerId = app.CustomerId,
                CustomerName = app.PersonalDetails?.FullName ?? "N/A",
                CustomerEmail = app.AddressInformation?.EmailId ?? "N/A",
                CustomerPhone = app.AddressInformation?.MobileNumber1 ?? "N/A",
                LoanType = app.LoanProductType.ToString(),
                RequestedAmount = app.RequestedLoanAmount,
                TenureMonths = app.TenureMonths,
                InterestRate = app.InterestRate,
                Status = app.Status.ToString(),
                SubmissionDate = app.SubmissionDate.ToDateTime(TimeOnly.MinValue),
                ApprovedDate = app.ApprovedDate?.ToDateTime(TimeOnly.MinValue),
                RejectionReason = app.RejectionReason,
                EMIStatus = emiPlan != null ? new EMIStatusDTO
                {
                    EMIId = emiPlan.EMIId,
                    MonthlyEMI = emiPlan.MonthlyEMI,
                    TotalRepaymentAmount = emiPlan.TotalRepaymentAmount,
                    TotalInterestPaid = emiPlan.TotalInterestPaid,
                    TermMonths = emiPlan.TermMonths,
                    Status = emiPlan.Status.ToString(),
                    IsCompleted = emiPlan.IsCompleted
                } : null,
                Documents = app.DocumentUpload != null ? new List<DocumentDTO>
                {
                    new DocumentDTO
                    {
                        DocumentId = app.DocumentUpload.DocumentId,
                        DocumentName = app.DocumentUpload.DocumentName,
                        DocumentType = app.DocumentUpload.DocumentType.ToString(),
                        UploadedAt = app.DocumentUpload.UploadedAt,
                        DocumentData = app.DocumentUpload.DocumentData
                    }
                } : new List<DocumentDTO>()
            };
        }
    }
}
