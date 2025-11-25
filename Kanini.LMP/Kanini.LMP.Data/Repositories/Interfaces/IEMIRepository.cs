using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.EntitiesDtos.CustomerEntitiesDtos;

namespace Kanini.LMP.Data.Repositories.Interfaces
{
    public interface IEMIRepository : ILMPRepository<EMIPlan, int>
    {
        Task<CustomerEMIDashboardDto?> GetCustomerEMIDashboardAsync(int customerId);
        Task<List<CustomerEMIDashboardDto>> GetAllCustomerEMIsAsync(int customerId);
        Task<List<PaymentTransaction>> GetPaymentsByEMIIdAsync(int emiId);
        Task<DateTime> GetLoanStartDateAsync(int loanApplicationBaseId);
        Task<decimal> GetTotalPaidAmountAsync(int emiId);
        Task<int> GetPaidInstallmentsCountAsync(int emiId);
    }
}