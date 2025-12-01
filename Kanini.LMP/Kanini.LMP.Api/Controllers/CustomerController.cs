using AutoMapper;
using Kanini.LMP.Api.Constants;
using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.Customer;
using Kanini.LMP.Database.EntitiesDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kanini.LMP.Api.Controllers
{
    [Route(ApiConstants.Routes.ApiController)]
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
        public async Task<ActionResult<ApiResponse<IReadOnlyList<CustomerDto>>>> GetAllCustomers()
        {
            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.GettingAllCustomers);
                var customers = await _customerService.GetAll();
                var customerList = customers.ToList();
                _logger.LogInformation(ApiConstants.LogMessages.RetrievedCustomers, customerList.Count);
                return Ok(ApiResponse<IReadOnlyList<CustomerDto>>.SuccessResponse(customerList));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiConstants.LogMessages.ErrorRetrievingAllCustomers);
                return BadRequest(ApiResponse<IReadOnlyList<CustomerDto>>.ErrorResponse(ApplicationConstants.ErrorMessages.UsersRetrievalFailed));
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = ApplicationConstants.Roles.Manager)]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> GetCustomer(int id)
        {
            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.GettingCustomerById, id);
                var customer = await _customerService.GetById(id);
                if (customer == null)
                {
                    _logger.LogWarning(ApiConstants.LogMessages.CustomerNotFoundById, id);
                    return NotFound(ApiResponse<CustomerDto>.ErrorResponse(ApplicationConstants.ErrorMessages.CustomerNotFound));
                }
                return Ok(ApiResponse<CustomerDto>.SuccessResponse(customer));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiConstants.LogMessages.ErrorRetrievingCustomerById, id);
                return BadRequest(ApiResponse<CustomerDto>.ErrorResponse(ApplicationConstants.ErrorMessages.UserRetrievalFailed));
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> GetCustomerByUserId(int userId)
        {
            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.GettingCustomerByUserId, userId);

                var currentUserId = GetCurrentUserId();
                var userRole = GetCurrentUserRole();

                if (userRole != ApplicationConstants.Roles.Manager && currentUserId != userId)
                {
                    _logger.LogWarning(ApiConstants.LogMessages.UnauthorizedProfileAccess, currentUserId, userId);
                    return StatusCode(403, ApiResponse<CustomerDto>.ErrorResponse(ApplicationConstants.ErrorMessages.AccessDenied));
                }

                var customer = await _customerService.GetByUserIdAsync(userId);
                if (customer == null)
                {
                    _logger.LogWarning(ApiConstants.LogMessages.CustomerNotFoundByUserId, userId);
                    return NotFound(ApiResponse<CustomerDto>.ErrorResponse(ApplicationConstants.ErrorMessages.CustomerNotFound));
                }

                return Ok(ApiResponse<CustomerDto>.SuccessResponse(customer));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiConstants.LogMessages.ErrorRetrievingCustomerByUserId, userId);
                return BadRequest(ApiResponse<CustomerDto>.ErrorResponse(ApplicationConstants.ErrorMessages.UserRetrievalFailed));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> CreateCustomer(CustomerDto customerDto)
        {
            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.CreatingCustomerProfile, customerDto.UserId);

                var currentUserId = GetCurrentUserId();
                var userRole = GetCurrentUserRole();

                if (userRole != ApplicationConstants.Roles.Manager && customerDto.UserId != currentUserId)
                {
                    _logger.LogWarning(ApiConstants.LogMessages.UnauthorizedProfileCreation, currentUserId, customerDto.UserId);
                    return StatusCode(403, ApiResponse<CustomerDto>.ErrorResponse(ApplicationConstants.ErrorMessages.CreateProfileDenied));
                }

                await _unitOfWork.BeginTransactionAsync();

                var created = await _customerService.Add(customerDto);
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation(ApiConstants.LogMessages.CustomerProfileCreated, created.CustomerId);
                return CreatedAtAction(nameof(GetCustomer), new { id = created.CustomerId }, ApiResponse<CustomerDto>.SuccessResponse(created, ApplicationConstants.Messages.Created));
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, ApiConstants.LogMessages.ErrorCreatingCustomerProfile, customerDto.UserId);
                return BadRequest(ApiResponse<CustomerDto>.ErrorResponse(ApplicationConstants.ErrorMessages.UserCreationFailed));
            }
        }

        [HttpPut("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> UpdateCustomer(int id, CustomerDto customerDto)
        {
            try
            {
                if (id != customerDto.CustomerId)
                {
                    _logger.LogWarning(ApiConstants.LogMessages.IdMismatch, id, customerDto.CustomerId);
                    return BadRequest(ApiResponse<CustomerDto>.ErrorResponse(ApplicationConstants.Messages.BadRequest));
                }

                _logger.LogInformation(ApiConstants.LogMessages.UpdatingCustomerProfile, id);

                var currentUserId = GetCurrentUserId();
                var userRole = GetCurrentUserRole();

                if (userRole != ApplicationConstants.Roles.Manager && customerDto.UserId != currentUserId)
                {
                    _logger.LogWarning(ApiConstants.LogMessages.UnauthorizedProfileUpdate, currentUserId, customerDto.UserId);
                    return StatusCode(403, ApiResponse<CustomerDto>.ErrorResponse(ApplicationConstants.ErrorMessages.UpdateProfileDenied));
                }

                await _unitOfWork.BeginTransactionAsync();

                var updated = await _customerService.Update(customerDto);
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation(ApiConstants.LogMessages.CustomerProfileUpdated, id);
                return Ok(ApiResponse<CustomerDto>.SuccessResponse(updated, ApplicationConstants.Messages.Updated));
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, ApiConstants.LogMessages.ErrorUpdatingCustomerProfile, id);
                return BadRequest(ApiResponse<CustomerDto>.ErrorResponse(ApplicationConstants.ErrorMessages.UserCreationFailed));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = ApplicationConstants.Roles.Manager)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<ApiResponse<object>>> DeleteCustomer(int id)
        {
            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.DeletingCustomer, id);

                await _unitOfWork.BeginTransactionAsync();

                await _customerService.Delete(id);
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation(ApiConstants.LogMessages.CustomerDeleted, id);
                return Ok(ApiResponse<object>.SuccessResponse(new { }, ApplicationConstants.Messages.Deleted));
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, ApiConstants.LogMessages.ErrorDeletingCustomer, id);
                return BadRequest(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.UserCreationFailed));
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogWarning(ApiConstants.LogMessages.InvalidUserIdClaim, userIdClaim);
                throw new UnauthorizedAccessException(ApplicationConstants.ErrorMessages.InvalidUserId);
            }
            return userId;
        }

        private string GetCurrentUserRole()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(role))
            {
                _logger.LogWarning(ApiConstants.LogMessages.NoRoleClaimFound);
                return string.Empty;
            }
            return role;
        }
    }
}