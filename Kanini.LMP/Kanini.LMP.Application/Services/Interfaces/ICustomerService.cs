using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface ICustomerService : ILMPService<CustomerDto, int>
    {
        Task<CustomerDto?> GetByUserIdAsync(int userId);
    }
}



