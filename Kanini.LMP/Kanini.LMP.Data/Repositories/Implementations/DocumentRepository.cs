using Kanini.LMP.Data.Data;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
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
}