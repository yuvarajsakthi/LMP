using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities;
using Microsoft.EntityFrameworkCore.Storage;

namespace Kanini.LMP.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ILMPRepository<User, int> Users { get; }
        ILMPRepository<Customer, int> Customers { get; }
        ILMPRepository<Notification, int> Notifications { get; }
        ILMPRepository<LoanApplicationBase, int> LoanApplications { get; }
        ILMPRepository<EMIPlan, int> EMIPlans { get; }
        ILMPRepository<LoanProduct, int> LoanProducts { get; }
        ILMPRepository<Faq, int> Faqs { get; }

        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}