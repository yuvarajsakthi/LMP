using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;

namespace Kanini.LMP.Data.Repositories.Interfaces
{
    public interface IDocumentRepository : ILMPRepository<DocumentUpload, int>
    {
        Task<(byte[] fileData, string fileName)> GetDocumentWithNameAsync(int documentId);
        Task<IReadOnlyList<DocumentUpload>> GetDocumentsByApplicationAsync(int loanApplicationBaseId);
        Task<bool> IsDocumentOwnedByUserAsync(int documentId, int userId);
    }
}