using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using Kanini.LMP.Database.Entities.CustomerEntities.JunctionTable;

namespace Kanini.LMP.Data.Repositories.Interfaces
{
    public interface IDocumentRepository : ILMPRepository<DocumentUpload, int>
    {
        Task<(byte[] fileData, string fileName)> GetDocumentWithNameAsync(int documentId);
        Task<IReadOnlyList<DocumentUpload>> GetDocumentsByApplicationAsync(int loanApplicationBaseId);
        Task<bool> IsDocumentOwnedByUserAsync(int documentId, int userId);
    }

    public interface IApplicationDocumentLinkRepository : ILMPRepository<ApplicationDocumentLink, int>
    {
        Task<IReadOnlyList<ApplicationDocumentLink>> GetLinksByApplicationAsync(int loanApplicationBaseId);
        Task<ApplicationDocumentLink?> GetLinkByApplicationAndDocumentAsync(int loanApplicationBaseId, int documentId);
        Task<IReadOnlyList<ApplicationDocumentLink>> GetPendingLinksAsync();
    }
}