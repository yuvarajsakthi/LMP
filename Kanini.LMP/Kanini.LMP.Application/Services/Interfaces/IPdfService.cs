using Kanini.LMP.Database.EntitiesDtos.Common;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface IPdfService
    {
        Task<ByteArrayDTO> GenerateLoanApplicationPdfAsync(IdDTO applicationId);
    }
}