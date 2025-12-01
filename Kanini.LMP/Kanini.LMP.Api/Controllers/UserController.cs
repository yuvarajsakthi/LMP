using Kanini.LMP.Api.Constants;
using Kanini.LMP.Application.Constants;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.EntitiesDto;
using Kanini.LMP.Database.EntitiesDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kanini.LMP.Api.Controllers
{
    [Route(ApiConstants.Routes.UserController.Base)]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserController> _logger;

        public UserController(IUser userService, IUnitOfWork unitOfWork, ILogger<UserController> logger)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<UserDTO>>>> GetAllUsers()
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingUsersRetrieval);
                var users = await _userService.GetAllUsersAsync();
                _logger.LogInformation(ApplicationConstants.Messages.UsersRetrievalCompleted, users.Count);
                return Ok(ApiResponse<IReadOnlyList<UserDTO>>.SuccessResponse(users));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.UsersRetrievalFailed);
                return BadRequest(ApiResponse<IReadOnlyList<UserDTO>>.ErrorResponse(ApplicationConstants.ErrorMessages.UsersRetrievalFailed));
            }
        }

        [HttpGet(ApiConstants.Routes.UserController.GetById)]
        [AllowAnonymous]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingUserRetrieval, id);
                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                {
                    _logger.LogWarning(ApplicationConstants.Messages.UserNotFoundWarning, id);
                    return NotFound(new { message = ApplicationConstants.ErrorMessages.UserNotFound });
                }

                _logger.LogInformation(ApplicationConstants.Messages.UserRetrievalCompleted, id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.UserRetrievalFailed);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.UserRetrievalFailed });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<UserDTO>>> CreateUser(UserDTO userDto)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingUserCreation, userDto.Email);
                
                await _unitOfWork.BeginTransactionAsync();
                var created = await _userService.CreateUserAsync(userDto);
                await _unitOfWork.CommitTransactionAsync();
                
                _logger.LogInformation(ApplicationConstants.Messages.UserCreationCompleted, created.UserId);
                return CreatedAtAction(nameof(GetUser), new { id = created.UserId }, ApiResponse<UserDTO>.SuccessResponse(created, ApplicationConstants.Messages.Created));
            }
            catch (ArgumentException ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogWarning(ex, ApplicationConstants.ErrorMessages.UserCreationValidationFailed);
                return BadRequest(ApiResponse<UserDTO>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.UserCreationFailed);
                return BadRequest(ApiResponse<UserDTO>.ErrorResponse(ApplicationConstants.ErrorMessages.UserCreationFailed));
            }
        }

        [HttpPost(ApiConstants.Routes.UserController.ChangePassword)]
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingPasswordChange, userId);

                var success = await _userService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);

                if (!success)
                {
                    _logger.LogWarning(ApplicationConstants.Messages.PasswordChangeIncorrectCurrent, userId);
                    return BadRequest(new { message = ApplicationConstants.ErrorMessages.CurrentPasswordIncorrect });
                }

                _logger.LogInformation(ApplicationConstants.Messages.PasswordChangeCompleted, userId);
                return Ok(new { message = ApplicationConstants.Messages.PasswordChangedSuccessfully });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, ApplicationConstants.ErrorMessages.PasswordChangeValidationFailed);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.PasswordChangeFailed);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.PasswordChangeFailed });
            }
        }
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
