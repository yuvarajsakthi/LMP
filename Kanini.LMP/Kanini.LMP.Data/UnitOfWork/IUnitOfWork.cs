using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.CustomerEntities.JunctionTable;
using Kanini.LMP.Database.Entities.ManagerEntities;
using Microsoft.EntityFrameworkCore.Storage;

namespace Kanini.LMP.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ILMPRepository<User, int> Users { get; }
        ILMPRepository<Customer, int> Customers { get; }
        INotificationRepository Notifications { get; }
        INotificationPreferenceRepository NotificationPreferences { get; }
        ILMPRepository<LoanApplicationBase, int> LoanApplications { get; }
        ILMPRepository<LoanAccount, int> LoanAccounts { get; }
        ILMPRepository<PaymentTransaction, int> PaymentTransactions { get; }
        ILMPRepository<EMIPlan, int> EMIPlans { get; }
        ILMPRepository<ApplicationDocumentLink, int> ApplicationDocuments { get; }
        ILMPRepository<LoanOriginationWorkflow, int> LoanWorkflows { get; }
        ILMPRepository<LoanApplicant, int> LoanApplicants { get; }

        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}