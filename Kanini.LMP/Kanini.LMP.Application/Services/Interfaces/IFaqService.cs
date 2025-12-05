using Kanini.LMP.Database.EntitiesDto;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface IFaqService
    {
        Task<FaqDTO> Add(FaqDTO entity);
        Task<IReadOnlyList<FaqDTO>> GetAll();
        Task<FaqDTO?> GetById(int id);
        Task<IReadOnlyList<FaqDTO>> GetByCustomerId(int customerId);
        Task<FaqDTO> Update(FaqDTO entity);
        Task Delete(int id);
    }
}