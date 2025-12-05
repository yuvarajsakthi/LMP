using Kanini.LMP.Data.Data;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Data.Repositories.Implementations;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.Entities.LoanProductEntities;
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
            PersonalLoanApplications = new LMPRepositoy<PersonalLoanApplication, int>(_context);
            HomeLoanApplications = new LMPRepositoy<HomeLoanApplication, int>(_context);
            VehicleLoanApplications = new LMPRepositoy<VehicleLoanApplication, int>(_context);
            EMIPlans = new LMPRepositoy<EMIPlan, int>(_context);
            LoanProducts = new LMPRepositoy<LoanProduct, int>(_context);
            Faqs = new LMPRepositoy<Faq, int>(_context);
        }

        public ILMPRepository<User, int> Users { get; }
        public ILMPRepository<Customer, int> Customers { get; }
        public ILMPRepository<Notification, int> Notifications { get; }
        public ILMPRepository<PersonalLoanApplication, int> PersonalLoanApplications { get; }
        public ILMPRepository<HomeLoanApplication, int> HomeLoanApplications { get; }
        public ILMPRepository<VehicleLoanApplication, int> VehicleLoanApplications { get; }
        public ILMPRepository<EMIPlan, int> EMIPlans { get; }
        public ILMPRepository<LoanProduct, int> LoanProducts { get; }
        public ILMPRepository<Faq, int> Faqs { get; }

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