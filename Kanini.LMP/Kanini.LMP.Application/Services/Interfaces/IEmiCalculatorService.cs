using Kanini.LMP.Database.EntitiesDtos.Common;
using Kanini.LMP.Database.EntitiesDtos.EMIPlanDtos;
using Kanini.LMP.Database.Enums;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface IEmiCalculatorService
    {
        Task<EMIPlanDTO> CalculateAndCreateEmiPlanAsync(IdDTO loanApplicationId, IdDTO customerId, decimal principalAmount, decimal interestRate, int termMonths, LoanType loanType);
        Task<EMIPlanDTO> CreateEmiPlanOnDisbursementAsync(IdDTO loanApplicationId);
    }
}
