using Kanini.LMP.Database.EntitiesDto.PaymentTransaction;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.EMIPlan;
using EntityPaymentStatus = Kanini.LMP.Database.Enums.PaymentStatus;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentTransactionDTO> CreatePaymentAsync(PaymentTransactionCreateDTO dto);
        Task<IReadOnlyList<PaymentTransactionDTO>> GetPaymentsByLoanAccountAsync(int loanAccountId);
        Task<PaymentTransactionDTO> UpdatePaymentStatusAsync(int paymentId, EntityPaymentStatus status);
        Task<IReadOnlyList<EMIPlanDTO>> GetEMIPlansByLoanAccountAsync(int loanAccountId);
        Task<PaymentAnalyticsResult> GetPaymentsByDateRangeViaSPAsync(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<PaymentTransactionDTO>> GetPaymentHistoryViaSPAsync(int loanAccountId);
    }

    public class PaymentAnalyticsResult
    {
        public decimal TotalAmount { get; set; }
        public int Count { get; set; }
        public decimal AverageAmount { get; set; }
        public decimal OnTimeAmount { get; set; }
        public decimal LateAmount { get; set; }
        public decimal PrepaymentAmount { get; set; }
        public decimal CollectionRate { get; set; }
    }
}
