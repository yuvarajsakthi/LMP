using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.ManagerEntities;

namespace Kanini.LMP.Data.Repositories.Interfaces
{
    public interface IPdfRepository
    {
        Task<LoanApplicationBase?> GetLoanApplicationWithDetailsAsync(int applicationId);
        Task<PaymentTransaction?> GetPaymentTransactionAsync(int transactionId);
        Task<LoanAccount?> GetLoanAccountAsync(int loanAccountId);
    }
}