using Kanini.LMP.Data.Data;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using Kanini.LMP.Database.Entities.CustomerEntities.JunctionTable;
using Kanini.LMP.Database.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kanini.LMP.Data.Repositories.Implementations
{
    public class DocumentRepository : LMPRepositoy<DocumentUpload, int>, IDocumentRepository
    {
        public DocumentRepository(LmpDbContext context) : base(context) { }

        public async Task<(byte[] fileData, string fileName)> GetDocumentWithNameAsync(int documentId)
        {
            var document = await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(d => d.DocumentId == documentId);

            if (document?.DocumentData == null)
                throw new FileNotFoundException("Document not found");

            return (document.DocumentData, document.DocumentName);
        }

        public async Task<IReadOnlyList<DocumentUpload>> GetDocumentsByApplicationAsync(int loanApplicationBaseId)
        {
            return await _dbSet.AsNoTracking()
                .Where(d => d.LoanApplicationBaseId == loanApplicationBaseId)
                .ToListAsync();
        }

        public async Task<bool> IsDocumentOwnedByUserAsync(int documentId, int userId)
        {
            return await _dbSet.AsNoTracking()
                .AnyAsync(d => d.DocumentId == documentId && d.UserId == userId);
        }
    }

    public class ApplicationDocumentLinkRepository : LMPRepositoy<ApplicationDocumentLink, int>, IApplicationDocumentLinkRepository
    {
        public ApplicationDocumentLinkRepository(LmpDbContext context) : base(context) { }

        public async Task<IReadOnlyList<ApplicationDocumentLink>> GetLinksByApplicationAsync(int loanApplicationBaseId)
        {
            return await _dbSet.AsNoTracking()
                .Where(l => l.LoanApplicationBaseId == loanApplicationBaseId)
                .ToListAsync();
        }

        public async Task<ApplicationDocumentLink?> GetLinkByApplicationAndDocumentAsync(int loanApplicationBaseId, int documentId)
        {
            return await _dbSet.FirstOrDefaultAsync(l =>
                l.LoanApplicationBaseId == loanApplicationBaseId &&
                l.DocumentId == documentId);
        }

        public async Task<IReadOnlyList<ApplicationDocumentLink>> GetPendingLinksAsync()
        {
            return await _dbSet.AsNoTracking()
                .Where(l => l.Status == DocumentStatus.Pending)
                .ToListAsync();
        }
    }
}