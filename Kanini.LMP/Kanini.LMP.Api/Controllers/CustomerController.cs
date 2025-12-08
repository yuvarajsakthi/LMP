using Kanini.LMP.Api.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDtos;
using Kanini.LMP.Database.EntitiesDtos.Common;
using Kanini.LMP.Database.EntitiesDtos.CustomerDtos;
using Kanini.LMP.Database.EntitiesDtos.UserDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.LMP.Api.Controllers
{
    [Route(ApiConstants.Routes.ApiController)]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IUserService _userService;

        public CustomerController(ICustomerService customerService, IUserService userService)
        {
            _customerService = customerService;
            _userService = userService;
        }

        [HttpGet(ApiConstants.Routes.CustomerController.GetAll)]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<CustomerDTO>>>> GetCustomers()
        {
            try
            {
                var customers = await _customerService.GetAll();
                return Ok(ApiResponse<IReadOnlyList<CustomerDTO>>.SuccessResponse(customers));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<IReadOnlyList<CustomerDTO>>.ErrorResponse("Failed to retrieve customers"));
            }
        }

        [HttpGet(ApiConstants.Routes.CustomerController.GetById)]
        public async Task<ActionResult<ApiResponse<CustomerDTO>>> GetCustomerById(int id)
        {
            try
            {
                var customer = await _customerService.GetById(new IdDTO { Id = id });
                if (customer == null)
                    return NotFound(ApiResponse<CustomerDTO>.ErrorResponse("Customer not found"));
                
                return Ok(ApiResponse<CustomerDTO>.SuccessResponse(customer));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<CustomerDTO>.ErrorResponse("Failed to retrieve customer"));
            }
        }

        [HttpGet(ApiConstants.Routes.CustomerController.GetByUserId)]
        public async Task<ActionResult<ApiResponse<CustomerDTO>>> GetCustomerByUserId(int userId)
        {
            try
            {
                var customer = await _customerService.GetByUserIdAsync(new IdDTO { Id = userId });
                if (customer == null)
                    return NotFound(ApiResponse<CustomerDTO>.ErrorResponse("Customer not found"));
                
                return Ok(ApiResponse<CustomerDTO>.SuccessResponse(customer));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<CustomerDTO>.ErrorResponse("Failed to retrieve customer"));
            }
        }

        [HttpGet(ApiConstants.Routes.CustomerController.GetSettings)]
        public async Task<ActionResult<ApiResponse<object>>> GetSettings(int userId)
        {
            try
            {
                var customer = await _customerService.GetByUserIdAsync(new IdDTO { Id = userId });
                if (customer == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("Customer not found"));

                var user = await _userService.GetUserByIdAsync(new IdDTO { Id = userId });
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

        [HttpPut(ApiConstants.Routes.CustomerController.Update)]
        public async Task<ActionResult<ApiResponse<CustomerDTO>>> UpdateCustomer(IdDTO id, [FromForm] CustomerUpdateDTO customerDto, IFormFile? profileImage)
        {
            try
            {
                if (profileImage != null)
                {
                    using var ms = new MemoryStream();
                    await profileImage.CopyToAsync(ms);
                    customerDto.ProfileImage = ms.ToArray();
                }

                var updated = await _customerService.Update(id, customerDto);
                return Ok(ApiResponse<CustomerDTO>.SuccessResponse(updated));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<CustomerDTO>.ErrorResponse("Failed to update customer"));
            }
        }

        [HttpPut(ApiConstants.Routes.CustomerController.UpdateSettings)]
        public async Task<ActionResult<ApiResponse<object>>> UpdateSettings(int userId, [FromForm] CustomerUpdateDTO settingsDto, IFormFile? profileImage)
        {
            try
            {
                var customer = await _customerService.GetByUserIdAsync(new IdDTO { Id = userId });
                if (customer == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("Customer not found"));

                var user = await _userService.GetUserByIdAsync(new IdDTO { Id = userId });
                if (user == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("User not found"));

                var userUpdate = new UserUpdateDTO
                {
                    UserId = userId,
                    FullName = user.FullName,
                    Email = user.Email
                };
                await _userService.UpdateUserAsync(userUpdate);

                if (profileImage != null)
                {
                    using var ms = new MemoryStream();
                    await profileImage.CopyToAsync(ms);
                    settingsDto.ProfileImage = ms.ToArray();
                }

                await _customerService.Update(new IdDTO { Id = customer.CustomerId }, settingsDto);

                return Ok(ApiResponse<object>.SuccessResponse(new { message = "Settings updated successfully" }));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to update settings"));
            }
        }
    }
}
