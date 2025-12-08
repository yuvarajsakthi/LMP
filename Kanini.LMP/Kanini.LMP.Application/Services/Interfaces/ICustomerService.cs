using Kanini.LMP.Database.EntitiesDtos.CustomerDtos;
using Kanini.LMP.Database.EntitiesDtos.Common;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerDTO?> GetByUserIdAsync(IdDTO request);
        Task<IReadOnlyList<CustomerDTO>> GetAll();
        Task<CustomerDTO?> GetById(IdDTO request);
        Task<CustomerDTO> Add(CustomerCreateDTO entity);
        Task<CustomerDTO> Update(IdDTO customerId, CustomerUpdateDTO entity);
        Task Delete(IdDTO request);
    }
}



