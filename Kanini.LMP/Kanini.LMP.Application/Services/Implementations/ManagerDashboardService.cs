using AutoMapper;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.EntitiesDtos;
using Kanini.LMP.Database.EntitiesDtos.EMIPlanDtos;
using Kanini.LMP.Database.Enums;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class ManagerDashboardService : IManagerDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ManagerDashboardService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
                    TotalAmount = g.Sum(a => a.RequestedAmount)
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
                TotalDisbursedAmount = allApplications.Where(a => a.Status == ApplicationStatus.Disbursed).Sum(a => a.RequestedAmount),
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
                if (!string.IsNullOrEmpty(dto.RejectionReason)) personalLoan.RejectionReason = dto.RejectionReason;
                if (newStatus == ApplicationStatus.Approved)
                {
                    personalLoan.ApprovedDate = DateOnly.FromDateTime(DateTime.UtcNow);
                    var interestRate = dto.InterestRate ?? CalculateInterestRate(personalLoan.LoanProductType, personalLoan.RequestedAmount);
                    personalLoan.MonthlyInstallment = CalculateEMI(personalLoan.RequestedAmount, interestRate, personalLoan.TenureMonths);
                }
                await _unitOfWork.PersonalLoanApplications.UpdateAsync(personalLoan);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }

            var homeLoan = await _unitOfWork.HomeLoanApplications.GetByIdAsync(dto.LoanApplicationBaseId);
            if (homeLoan != null)
            {
                homeLoan.Status = newStatus;
                if (!string.IsNullOrEmpty(dto.RejectionReason)) homeLoan.RejectionReason = dto.RejectionReason;
                if (newStatus == ApplicationStatus.Approved)
                {
                    homeLoan.ApprovedDate = DateOnly.FromDateTime(DateTime.UtcNow);
                    var interestRate = dto.InterestRate ?? CalculateInterestRate(homeLoan.LoanProductType, homeLoan.RequestedAmount);
                    homeLoan.MonthlyInstallment = CalculateEMI(homeLoan.RequestedAmount, interestRate, homeLoan.TenureMonths);
                }
                await _unitOfWork.HomeLoanApplications.UpdateAsync(homeLoan);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }

            var vehicleLoan = await _unitOfWork.VehicleLoanApplications.GetByIdAsync(dto.LoanApplicationBaseId);
            if (vehicleLoan != null)
            {
                vehicleLoan.Status = newStatus;
                if (!string.IsNullOrEmpty(dto.RejectionReason)) vehicleLoan.RejectionReason = dto.RejectionReason;
                if (newStatus == ApplicationStatus.Approved)
                {
                    vehicleLoan.ApprovedDate = DateOnly.FromDateTime(DateTime.UtcNow);
                    var interestRate = dto.InterestRate ?? CalculateInterestRate(vehicleLoan.LoanProductType, vehicleLoan.RequestedAmount);
                    vehicleLoan.MonthlyInstallment = CalculateEMI(vehicleLoan.RequestedAmount, interestRate, vehicleLoan.TenureMonths);
                }
                await _unitOfWork.VehicleLoanApplications.UpdateAsync(vehicleLoan);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> DisburseLoanAsync(int loanApplicationBaseId)
        {
            var personalLoan = await _unitOfWork.PersonalLoanApplications.GetByIdAsync(loanApplicationBaseId);
            if (personalLoan != null && personalLoan.Status == ApplicationStatus.Approved)
            {
                personalLoan.Status = ApplicationStatus.Disbursed;
                await _unitOfWork.PersonalLoanApplications.UpdateAsync(personalLoan);
                await CreateEMIPlanAsync(personalLoan);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }

            var homeLoan = await _unitOfWork.HomeLoanApplications.GetByIdAsync(loanApplicationBaseId);
            if (homeLoan != null && homeLoan.Status == ApplicationStatus.Approved)
            {
                homeLoan.Status = ApplicationStatus.Disbursed;
                await _unitOfWork.HomeLoanApplications.UpdateAsync(homeLoan);
                await CreateEMIPlanAsync(homeLoan);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }

            var vehicleLoan = await _unitOfWork.VehicleLoanApplications.GetByIdAsync(loanApplicationBaseId);
            if (vehicleLoan != null && vehicleLoan.Status == ApplicationStatus.Approved)
            {
                vehicleLoan.Status = ApplicationStatus.Disbursed;
                await _unitOfWork.VehicleLoanApplications.UpdateAsync(vehicleLoan);
                await CreateEMIPlanAsync(vehicleLoan);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }

            return false;
        }

        private async Task CreateEMIPlanAsync(LoanApplicationBase loan)
        {
            var interestRate = CalculateInterestRate(loan.LoanProductType, loan.RequestedAmount);
            var monthlyEMI = loan.MonthlyInstallment ?? CalculateEMI(loan.RequestedAmount, interestRate, loan.TenureMonths);
            var totalRepayment = monthlyEMI * loan.TenureMonths;
            var totalInterest = totalRepayment - loan.RequestedAmount;

            var createDto = new EMIPlanCreateDTO
            {
                LoanApplicationBaseId = loan.LoanApplicationBaseId,
                CustomerId = loan.CustomerId,
                PrincipleAmount = loan.RequestedAmount,
                TermMonths = loan.TenureMonths,
                RateOfInterest = interestRate,
                MonthlyEMI = monthlyEMI,
                TotalRepaymentAmount = totalRepayment,
                TotalInterestPaid = totalInterest
            };

            var emiPlan = _mapper.Map<EMIPlan>(createDto);
            emiPlan.Status = EMIPlanStatus.Active;
            emiPlan.IsCompleted = false;
            emiPlan.PaidInstallments = 0;
            emiPlan.NextPaymentDate = DateTime.UtcNow.AddMonths(1);

            await _unitOfWork.EMIPlans.AddAsync(emiPlan);
        }

        private decimal CalculateInterestRate(LoanType loanType, decimal amount)
        {
            return loanType switch
            {
                LoanType.Personal => amount > 500000 ? 10.5m : 12.0m,
                LoanType.Home => amount > 2000000 ? 8.5m : 9.0m,
                LoanType.Vehicle => amount > 1000000 ? 9.0m : 10.0m,
                _ => 10.0m
            };
        }

        private decimal CalculateEMI(decimal principal, decimal annualRate, int months)
        {
            if (annualRate == 0 || months == 0) return 0;
            var monthlyRate = annualRate / 12 / 100;
            if (monthlyRate == 0) return principal / months;
            return principal * monthlyRate * (decimal)Math.Pow((double)(1 + monthlyRate), months) / 
                   ((decimal)Math.Pow((double)(1 + monthlyRate), months) - 1);
        }

        // Helper Methods
        private async Task<LoanApplicationDetailDTO> MapToDetailDTO(LoanApplicationBase app)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(app.CustomerId);
            var user = customer != null ? await _unitOfWork.Users.GetByIdAsync(customer.UserId) : null;
            var emiPlans = (await _unitOfWork.EMIPlans.GetAllAsync()).Where(e => e.LoanApplicationBaseId == app.LoanApplicationBaseId);
            var emiPlan = emiPlans.FirstOrDefault();

            return new LoanApplicationDetailDTO
            {
                LoanApplicationBaseId = app.LoanApplicationBaseId,
                CustomerId = app.CustomerId,
                CustomerName = user?.FullName ?? "N/A",
                CustomerEmail = user?.Email ?? "N/A",
                CustomerPhone = customer?.PhoneNumber ?? "N/A",
                LoanType = app.LoanProductType.ToString(),
                RequestedAmount = app.RequestedAmount,
                TenureMonths = app.TenureMonths,
                InterestRate = 0,
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
                Documents = new List<DocumentDTO>()
            };
        }
    }
}
