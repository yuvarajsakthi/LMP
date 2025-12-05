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
                    personalLoan.RequestedLoanAmount,
                    personalLoan.InterestRate ?? 0,
                    personalLoan.TenureMonths,
                    LoanType.Personal);
            }

            var homeLoan = await _unitOfWork.HomeLoanApplications.GetByIdAsync(loanApplicationId.Id);
            if (homeLoan != null && homeLoan.Status == ApplicationStatus.Disbursed)
            {
                return await CalculateAndCreateEmiPlanAsync(
                    loanApplicationId,
                    new IdDTO { Id = homeLoan.CustomerId },
                    homeLoan.RequestedLoanAmount,
                    homeLoan.InterestRate ?? 0,
                    homeLoan.TenureMonths,
                    LoanType.Home);
            }

            var vehicleLoan = await _unitOfWork.VehicleLoanApplications.GetByIdAsync(loanApplicationId.Id);
            if (vehicleLoan != null && vehicleLoan.Status == ApplicationStatus.Disbursed)
            {
                return await CalculateAndCreateEmiPlanAsync(
                    loanApplicationId,
                    new IdDTO { Id = vehicleLoan.CustomerId },
                    vehicleLoan.RequestedLoanAmount,
                    vehicleLoan.InterestRate ?? 0,
                    vehicleLoan.TenureMonths,
                    LoanType.Vehicle);
            }

            throw new InvalidOperationException("Loan application not found or not in Disbursed status");
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
