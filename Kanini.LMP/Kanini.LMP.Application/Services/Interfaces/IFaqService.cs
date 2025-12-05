using Kanini.LMP.Database.EntitiesDto;
using Kanini.LMP.Database.EntitiesDtos.Common;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface IFaqService
    {
        Task<FaqDTO> Add(FaqDTO entity);
        Task<IReadOnlyList<FaqDTO>> GetAll();
        Task<FaqDTO?> GetById(IdDTO id);
        Task<IReadOnlyList<FaqDTO>> GetByCustomerId(IdDTO customerId);
        Task<FaqDTO> Update(FaqDTO entity);
        Task Delete(IdDTO id);
    }
}