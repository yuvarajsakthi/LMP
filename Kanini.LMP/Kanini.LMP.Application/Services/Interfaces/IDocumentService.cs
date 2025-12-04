using Kanini.LMP.Database.EntitiesDtos.DocumentDtos;
using Kanini.LMP.Database.Enums;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface IDocumentService
    {
        Task<IReadOnlyList<DocumentUploadDto>> GetDocumentsByApplicationAsync(int loanApplicationBaseId);
        Task<DocumentUploadDto> UploadDocumentAsync(DocumentUploadRequest request, int userId);
        Task<byte[]> DownloadDocumentAsync(int documentId, int userId);
        Task<bool> DeleteDocumentAsync(int documentId, int userId);
    }
}