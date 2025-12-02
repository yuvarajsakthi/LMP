using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.Customer;
using Kanini.LMP.Database.EntitiesDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.LMP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<CustomerDto>>>> GetCustomers()
        {
            try
            {
                var customers = await _customerService.GetAll();
                return Ok(ApiResponse<IReadOnlyList<CustomerDto>>.SuccessResponse(customers.ToList()));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<IReadOnlyList<CustomerDto>>.ErrorResponse("Failed to retrieve customers"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> GetCustomerById(int id)
        {
            try
            {
                var customer = await _customerService.GetById(id);
                if (customer == null)
                    return NotFound(ApiResponse<CustomerDto>.ErrorResponse("Customer not found"));
                
                return Ok(ApiResponse<CustomerDto>.SuccessResponse(customer));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<CustomerDto>.ErrorResponse("Failed to retrieve customer"));
            }
        }



        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> UpdateCustomer(int id, CustomerDto customerDto)
        {
            try
            {
                if (id != customerDto.CustomerId)
                    return BadRequest(ApiResponse<CustomerDto>.ErrorResponse("ID mismatch"));

                var updated = await _customerService.Update(customerDto);
                return Ok(ApiResponse<CustomerDto>.SuccessResponse(updated));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<CustomerDto>.ErrorResponse("Failed to update customer"));
            }
        }


    }
}