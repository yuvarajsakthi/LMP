using Kanini.LMP.Data.Data;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Data.Repositories.Implementations;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.CustomerEntities.JunctionTable;
using Kanini.LMP.Database.Entities.LoanProductEntities;
using Kanini.LMP.Database.Entities.ManagerEntities;
using Microsoft.EntityFrameworkCore.Storage;

namespace Kanini.LMP.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LmpDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(LmpDbContext context)
        {
            _context = context;
            Users = new LMPRepositoy<User, int>(_context);
            Customers = new LMPRepositoy<Customer, int>(_context);
            Notifications = new LMPRepositoy<Notification, int>(_context);
            NotificationPreferences = new LMPRepositoy<NotificationPreference, int>(_context);
            LoanApplications = new LMPRepositoy<LoanApplicationBase, int>(_context);
            LoanAccounts = new LMPRepositoy<LoanAccount, int>(_context);
            PaymentTransactions = new LMPRepositoy<PaymentTransaction, int>(_context);
            EMIPlans = new LMPRepositoy<EMIPlan, int>(_context);
            ApplicationDocuments = new LMPRepositoy<ApplicationDocumentLink, int>(_context);
            LoanWorkflows = new LMPRepositoy<LoanOriginationWorkflow, int>(_context);
            LoanApplicants = new LMPRepositoy<LoanApplicant, int>(_context);
            LoanProducts = new LMPRepositoy<LoanProduct, int>(_context);
        }

        public ILMPRepository<User, int> Users { get; }
        public ILMPRepository<Customer, int> Customers { get; }
        public ILMPRepository<Notification, int> Notifications { get; }
        public ILMPRepository<NotificationPreference, int> NotificationPreferences { get; }
        public ILMPRepository<LoanApplicationBase, int> LoanApplications { get; }
        public ILMPRepository<LoanAccount, int> LoanAccounts { get; }
        public ILMPRepository<PaymentTransaction, int> PaymentTransactions { get; }
        public ILMPRepository<EMIPlan, int> EMIPlans { get; }
        public ILMPRepository<ApplicationDocumentLink, int> ApplicationDocuments { get; }
        public ILMPRepository<LoanOriginationWorkflow, int> LoanWorkflows { get; }
        public ILMPRepository<LoanApplicant, int> LoanApplicants { get; }
        public ILMPRepository<LoanProduct, int> LoanProducts { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
            return _transaction;
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}