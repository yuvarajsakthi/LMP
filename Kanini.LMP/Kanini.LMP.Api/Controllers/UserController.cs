using Kanini.LMP.Application.Constants;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.EntitiesDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kanini.LMP.Api.Controllers
{
    [Route(ApplicationConstants.Routes.UserController.Base)]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUser userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyList<UserDTO>>> GetAllUsers()
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingUsersRetrieval);
                var users = await _userService.GetAllUsersAsync();
                _logger.LogInformation(ApplicationConstants.Messages.UsersRetrievalCompleted, users.Count);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.UsersRetrievalFailed);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.UsersRetrievalFailed });
            }
        }

        [HttpGet(ApplicationConstants.Routes.UserController.GetById)]
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
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.UserRetrievalFailed, id);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.UserRetrievalFailed });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<UserDTO>> CreateUser(UserDTO userDto)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingUserCreation, userDto.Email);
                var created = await _userService.CreateUserAsync(userDto);
                _logger.LogInformation(ApplicationConstants.Messages.UserCreationCompleted, created.UserId);

                return CreatedAtAction(nameof(GetUser), new { id = created.UserId }, created);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, ApplicationConstants.ErrorMessages.UserCreationValidationFailed);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.UserCreationFailed);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.UserCreationFailed });
            }
        }

        [HttpPost(ApplicationConstants.Routes.UserController.ChangePassword)]
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
