using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerDto?> GetByUserIdAsync(int userId);
        Task<IReadOnlyList<CustomerDto>> GetAll();
        Task<CustomerDto?> GetById(int id);
        Task<CustomerDto> Add(CustomerDto entity);
        Task<CustomerDto> Update(CustomerDto entity);
        Task Delete(int id);
    }
}



