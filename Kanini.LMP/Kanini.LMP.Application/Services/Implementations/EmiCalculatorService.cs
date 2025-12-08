using AutoMapper;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.EntitiesDtos.Common;
using Kanini.LMP.Database.EntitiesDtos.EMIPlanDtos;
using Kanini.LMP.Database.Enums;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class EmiCalculatorService : IEmiCalculatorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmiCalculatorService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<EMIPlanDTO> CalculateAndCreateEmiPlanAsync(IdDTO loanApplicationId, IdDTO customerId, decimal principalAmount, decimal interestRate, int termMonths, LoanType loanType)
        {
            var adjustedRate = AdjustInterestRate(interestRate, loanType);
            var monthlyRate = adjustedRate / 12 / 100;
            var monthlyEmi = principalAmount * monthlyRate * (decimal)Math.Pow((double)(1 + monthlyRate), termMonths) / 
                            ((decimal)Math.Pow((double)(1 + monthlyRate), termMonths) - 1);
            var totalRepayment = monthlyEmi * termMonths;
            var totalInterest = totalRepayment - principalAmount;

            var createDto = new EMIPlanCreateDTO
            {
                LoanApplicationBaseId = loanApplicationId.Id,
                CustomerId = customerId.Id,
                PrincipleAmount = principalAmount,
                TermMonths = termMonths,
                RateOfInterest = adjustedRate,
                MonthlyEMI = decimal.Round(monthlyEmi, 2),
                TotalInterestPaid = decimal.Round(totalInterest, 2),
                TotalRepaymentAmount = decimal.Round(totalRepayment, 2)
            };

            var emiPlan = _mapper.Map<dynamic>(createDto);
            var created = await _unitOfWork.EMIPlans.AddAsync(emiPlan);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<EMIPlanDTO>(created);
        }

        public async Task<EMIPlanDTO> CreateEmiPlanOnDisbursementAsync(IdDTO loanApplicationId)
        {
            var personalLoan = await _unitOfWork.PersonalLoanApplications.GetByIdAsync(loanApplicationId.Id);
            if (personalLoan != null && personalLoan.Status == ApplicationStatus.Disbursed)
            {
                return await CalculateAndCreateEmiPlanAsync(
                    loanApplicationId,
                    new IdDTO { Id = personalLoan.CustomerId },
                    personalLoan.RequestedAmount,
                    10.5m,
                    personalLoan.TenureMonths,
                    LoanType.Personal);
            }

            var homeLoan = await _unitOfWork.HomeLoanApplications.GetByIdAsync(loanApplicationId.Id);
            if (homeLoan != null && homeLoan.Status == ApplicationStatus.Disbursed)
            {
                return await CalculateAndCreateEmiPlanAsync(
                    loanApplicationId,
                    new IdDTO { Id = homeLoan.CustomerId },
                    homeLoan.RequestedAmount,
                    8.5m,
                    homeLoan.TenureMonths,
                    LoanType.Home);
            }

            var vehicleLoan = await _unitOfWork.VehicleLoanApplications.GetByIdAsync(loanApplicationId.Id);
            if (vehicleLoan != null && vehicleLoan.Status == ApplicationStatus.Disbursed)
            {
                return await CalculateAndCreateEmiPlanAsync(
                    loanApplicationId,
                    new IdDTO { Id = vehicleLoan.CustomerId },
                    vehicleLoan.RequestedAmount,
                    9.5m,
                    vehicleLoan.TenureMonths,
                    LoanType.Vehicle);
            }

            throw new InvalidOperationException("Loan application not found or not in Disbursed status");
        }

        public async Task<EMIDashboardDTO> GetCustomerEMIDashboardAsync(IdDTO customerId)
        {
            var emiPlan = (await _unitOfWork.EMIPlans.GetAllAsync(e => e.CustomerId == customerId.Id && !e.IsCompleted)).FirstOrDefault();
            if (emiPlan == null)
                return null!;

            var dashboard = _mapper.Map<EMIDashboardDTO>(emiPlan);
            dashboard.NextPaymentDate = emiPlan.NextPaymentDate ?? DateTime.UtcNow;
            dashboard.CanPayNow = emiPlan.LastPaymentDate == null || DateTime.UtcNow >= emiPlan.LastPaymentDate.Value.AddMonths(1).AddDays(-5);

            return dashboard;
        }

        public async Task<bool> PayMonthlyEMIAsync(IdDTO emiId)
        {
            var emiPlan = await _unitOfWork.EMIPlans.GetByIdAsync(emiId.Id);
            if (emiPlan == null || emiPlan.IsCompleted)
                return false;

            if (emiPlan.LastPaymentDate != null && DateTime.UtcNow < emiPlan.LastPaymentDate.Value.AddMonths(1).AddDays(-5))
                return false;

            emiPlan.PaidInstallments++;
            emiPlan.LastPaymentDate = DateTime.UtcNow;
            emiPlan.NextPaymentDate = DateTime.UtcNow.AddMonths(1);

            if (emiPlan.PaidInstallments >= emiPlan.TermMonths)
            {
                emiPlan.IsCompleted = true;
                emiPlan.Status = EMIPlanStatus.Closed;
            }

            await _unitOfWork.EMIPlans.UpdateAsync(emiPlan);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private decimal AdjustInterestRate(decimal baseRate, LoanType loanType)
        {
            return loanType switch
            {
                LoanType.Home => baseRate - 0.5m,
                LoanType.Personal => baseRate,
                LoanType.Vehicle => baseRate + 0.3m,
                _ => baseRate
            };
        }
    }
}
