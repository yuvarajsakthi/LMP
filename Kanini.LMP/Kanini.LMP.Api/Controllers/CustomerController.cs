using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.Customer;
using Kanini.LMP.Database.EntitiesDtos;
using Kanini.LMP.Database.EntitiesDtos.CustomerEntitiesDtos;
using Kanini.LMP.Data.Repositories.Interfaces;
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
        private readonly IUser _userService;

        public CustomerController(ICustomerService customerService, IUser userService)
        {
            _customerService = customerService;
            _userService = userService;
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

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> GetCustomerByUserId(int userId)
        {
            try
            {
                var customer = await _customerService.GetByUserIdAsync(userId);
                if (customer == null)
                    return NotFound(ApiResponse<CustomerDto>.ErrorResponse("Customer not found"));
                
                return Ok(ApiResponse<CustomerDto>.SuccessResponse(customer));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<CustomerDto>.ErrorResponse("Failed to retrieve customer"));
            }
        }

        [HttpGet("settings/{userId}")]
        public async Task<ActionResult<ApiResponse<object>>> GetSettings(int userId)
        {
            try
            {
                var customer = await _customerService.GetByUserIdAsync(userId);
                if (customer == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("Customer not found"));

                var user = await _userService.GetByIdAsync(userId);
                if (user == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("User not found"));

                var settings = new
                {
                    fullName = user.FullName,
                    email = user.Email,
                    phoneNumber = customer.PhoneNumber,
                    occupation = customer.Occupation,
                    annualIncome = customer.AnnualIncome
                };

                return Ok(ApiResponse<object>.SuccessResponse(settings));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to retrieve settings"));
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

        [HttpPut("settings/{userId}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateSettings(int userId, UpdateCustomerSettingsDto settingsDto)
        {
            try
            {
                var customer = await _customerService.GetByUserIdAsync(userId);
                if (customer == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("Customer not found"));

                var user = await _userService.GetByIdAsync(userId);
                if (user == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("User not found"));

                user.FullName = settingsDto.FullName;
                await _userService.UpdateAsync(user);

                customer.PhoneNumber = settingsDto.PhoneNumber;
                customer.Occupation = settingsDto.Occupation;
                customer.AnnualIncome = settingsDto.AnnualIncome;
                await _customerService.Update(customer);

                return Ok(ApiResponse<object>.SuccessResponse(new { message = "Settings updated successfully" }));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to update settings"));
            }
        }

    }
}