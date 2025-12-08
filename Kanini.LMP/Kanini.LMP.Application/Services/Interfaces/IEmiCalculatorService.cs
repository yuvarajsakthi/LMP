using Kanini.LMP.Database.EntitiesDtos.Common;
using Kanini.LMP.Database.EntitiesDtos.EMIPlanDtos;
using Kanini.LMP.Database.Enums;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface IEmiCalculatorService
    {
        Task<EMIPlanDTO> CalculateAndCreateEmiPlanAsync(IdDTO loanApplicationId, IdDTO customerId, decimal principalAmount, decimal interestRate, int termMonths, LoanType loanType);
        Task<EMIPlanDTO> CreateEmiPlanOnDisbursementAsync(IdDTO loanApplicationId);
        Task<EMIDashboardDTO> GetCustomerEMIDashboardAsync(IdDTO customerId);
        Task<bool> PayMonthlyEMIAsync(IdDTO emiId);
    }

    public class EMIDashboardDTO
    {
        public int EMIId { get; set; }
        public decimal MonthlyEMI { get; set; }
        public decimal TotalInterestPaid { get; set; }
        public decimal RemainingAmount { get; set; }
        public int PaidInstallments { get; set; }
        public int RemainingInstallments { get; set; }
        public DateTime? NextPaymentDate { get; set; }
        public bool CanPayNow { get; set; }
    }
}
