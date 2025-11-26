using Kanini.LMP.Data.Data;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.ManagerEntities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.LMP.Data.Repositories.Implementations
{
    public class PdfRepository : IPdfRepository
    {
        private readonly LmpDbContext _context;

        public PdfRepository(LmpDbContext context)
        {
            _context = context;
        }

        public async Task<LoanApplicationBase?> GetLoanApplicationWithDetailsAsync(int applicationId)
        {
            return await _context.LoanApplicationBases
                .Include(app => app.LoanDetails)
                .FirstOrDefaultAsync(app => app.LoanApplicationBaseId == applicationId);
        }

        public async Task<PaymentTransaction?> GetPaymentTransactionAsync(int transactionId)
        {
            return await _context.PaymentTransactions
                .FirstOrDefaultAsync(pt => pt.TransactionId == transactionId);
        }

        public async Task<LoanAccount?> GetLoanAccountAsync(int loanAccountId)
        {
            return await _context.LoanAccounts
                .FirstOrDefaultAsync(la => la.LoanAccountId == loanAccountId);
        }
    }
}