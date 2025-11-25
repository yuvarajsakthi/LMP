using AutoMapper;
using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kanini.LMP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(
            ICustomerService customerService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = ApplicationConstants.Roles.Manager)]
        public async Task<ActionResult<IReadOnlyList<CustomerDto>>> GetAllCustomers()
        {
            try
            {
                _logger.LogInformation("Getting all customers");
                var customers = await _customerService.GetAll();
                _logger.LogInformation("Retrieved {Count} customers", customers.Count());
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all customers");
                throw;
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = ApplicationConstants.Roles.Manager)]
        public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
        {
            try
            {
                _logger.LogInformation("Getting customer with ID: {CustomerId}", id);
                var customer = await _customerService.GetById(id);
                if (customer == null)
                {
                    _logger.LogWarning("Customer with ID {CustomerId} not found", id);
                    return NotFound(ApplicationConstants.ErrorMessages.CustomerNotFound);
                }
                return Ok(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer with ID: {CustomerId}", id);
                throw;
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<CustomerDto>> GetCustomerByUserId(int userId)
        {
            try
            {
                _logger.LogInformation("Getting customer by user ID: {UserId}", userId);

                var currentUserId = GetCurrentUserId();
                var userRole = GetCurrentUserRole();

                if (userRole != ApplicationConstants.Roles.Manager && currentUserId != userId)
                {
                    _logger.LogWarning("User {CurrentUserId} attempted to access profile of user {UserId}", currentUserId, userId);
                    return Forbid(ApplicationConstants.ErrorMessages.AccessDenied);
                }

                var customer = await _customerService.GetByUserIdAsync(userId);
                if (customer == null)
                {
                    _logger.LogWarning("Customer with user ID {UserId} not found", userId);
                    return NotFound(ApplicationConstants.ErrorMessages.CustomerNotFound);
                }

                return Ok(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer by user ID: {UserId}", userId);
                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult<CustomerDto>> CreateCustomer(CustomerDto customerDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                _logger.LogInformation("Creating customer profile for user ID: {UserId}", customerDto.UserId);

                var currentUserId = GetCurrentUserId();
                var userRole = GetCurrentUserRole();

                if (userRole != ApplicationConstants.Roles.Manager && customerDto.UserId != currentUserId)
                {
                    _logger.LogWarning("User {CurrentUserId} attempted to create profile for user {UserId}", currentUserId, customerDto.UserId);
                    return Forbid(ApplicationConstants.ErrorMessages.CreateProfileDenied);
                }

                var created = await _customerService.Add(customerDto);
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Customer profile created successfully with ID: {CustomerId}", created.CustomerId);
                return CreatedAtAction(nameof(GetCustomer), new { id = created.CustomerId }, created);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating customer profile for user ID: {UserId}", customerDto.UserId);
                throw;
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerDto>> UpdateCustomer(int id, CustomerDto customerDto)
        {
            try
            {
                if (id != customerDto.CustomerId)
                {
                    _logger.LogWarning("ID mismatch: URL ID {UrlId} vs DTO ID {DtoId}", id, customerDto.CustomerId);
                    return BadRequest(ApplicationConstants.Messages.BadRequest);
                }

                await _unitOfWork.BeginTransactionAsync();

                _logger.LogInformation("Updating customer profile with ID: {CustomerId}", id);

                var currentUserId = GetCurrentUserId();
                var userRole = GetCurrentUserRole();

                if (userRole != ApplicationConstants.Roles.Manager && customerDto.UserId != currentUserId)
                {
                    _logger.LogWarning("User {CurrentUserId} attempted to update profile of user {UserId}", currentUserId, customerDto.UserId);
                    return Forbid(ApplicationConstants.ErrorMessages.UpdateProfileDenied);
                }

                var updated = await _customerService.Update(customerDto);
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Customer profile updated successfully with ID: {CustomerId}", id);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error updating customer profile with ID: {CustomerId}", id);
                throw;
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = ApplicationConstants.Roles.Manager)]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                _logger.LogInformation("Deleting customer with ID: {CustomerId}", id);

                await _customerService.Delete(id);
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Customer deleted successfully with ID: {CustomerId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error deleting customer with ID: {CustomerId}", id);
                throw;
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogWarning("Invalid user ID claim: {UserIdClaim}", userIdClaim);
                throw new UnauthorizedAccessException(ApplicationConstants.ErrorMessages.InvalidUserId);
            }
            return userId;
        }

        private string GetCurrentUserRole()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(role))
            {
                _logger.LogWarning("No role claim found for user");
                return string.Empty;
            }
            return role;
        }
    }
}