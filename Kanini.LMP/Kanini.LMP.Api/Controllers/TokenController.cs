using Kanini.LMP.Api.Constants;
using Kanini.LMP.Application.Constants;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.EntitiesDtos;
using Kanini.LMP.Database.EntitiesDtos.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.LMP.Api.Controllers
{
    [Route(ApiConstants.Routes.ApiController)]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUser _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TokenController> _logger;

        public TokenController(IUser userService, ITokenService tokenService, IUnitOfWork unitOfWork, ILogger<TokenController> logger)
        {
            _tokenService = tokenService;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpPost(ApiConstants.Routes.TokenController.Login)]
        public async Task<ActionResult<ApiResponse<object>>> Login([FromBody] LoginDTO loginDto)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingLogin, loginDto?.Username ?? "unknown");

                if (loginDto == null || string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
                {
                    _logger.LogWarning(ApplicationConstants.Messages.LoginValidationFailed);
                    return BadRequest(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.UsernamePasswordRequired));
                }

                var token = await _tokenService.AuthenticateAsync(loginDto.Username, loginDto.Password);

                if (token == null)
                {
                    _logger.LogWarning(ApplicationConstants.Messages.LoginFailed, loginDto.Username);
                    return Unauthorized(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.InvalidCredentials));
                }

                var user = await _userService.GetByUsernameAsync(loginDto.Username);
                if (user == null)
                {
                    _logger.LogWarning(ApplicationConstants.Messages.LoginFailed, loginDto.Username);
                    return Unauthorized(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.InvalidCredentials));
                }
                var loginResponse = new
                {
                    token,
                    username = user.FullName,
                    role = user.Roles.ToString()
                };

                _logger.LogInformation(ApplicationConstants.Messages.LoginCompleted, loginDto.Username);
                return Ok(ApiResponse<object>.SuccessResponse(loginResponse, ApplicationConstants.Messages.Success));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.LoginProcessingFailed);
                return BadRequest(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.LoginProcessingFailed));
            }
        }

        [HttpPost(ApiConstants.Routes.TokenController.Register)]
        public async Task<ActionResult<ApiResponse<object>>> RegisterCustomer([FromBody] CustomerRegistrationDTO registrationDto)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingRegistration, registrationDto?.Email ?? "unknown");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning(ApplicationConstants.Messages.RegistrationValidationFailed);
                    return BadRequest(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.RegistrationFailed, 
                        ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
                }

                await _unitOfWork.BeginTransactionAsync();
                var userDto = await _userService.RegisterCustomerAsync(registrationDto!);
                await _unitOfWork.CommitTransactionAsync();

                var registrationResponse = new
                {
                    message = ApplicationConstants.Messages.CustomerRegisteredSuccessfully,
                    userId = userDto.UserId,
                    email = userDto.Email,
                    fullName = userDto.FullName
                };

                _logger.LogInformation(ApplicationConstants.Messages.RegistrationCompleted, registrationDto?.Email ?? "unknown");
                return Ok(ApiResponse<object>.SuccessResponse(registrationResponse, ApplicationConstants.Messages.CustomerRegisteredSuccessfully));
            }
            catch (InvalidOperationException ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogWarning(ex, ApplicationConstants.ErrorMessages.RegistrationFailed);
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.RegistrationFailed);
                return BadRequest(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.RegistrationFailed));
            }
        }

        [HttpPost(ApiConstants.Routes.TokenController.ForgotPassword)]
        public async Task<ActionResult<ApiResponse<object>>> ForgotPassword([FromBody] ForgotPasswordRequest dto)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingForgotPassword, dto?.Email ?? "unknown");

                if (string.IsNullOrEmpty(dto?.Email))
                {
                    _logger.LogWarning(ApplicationConstants.Messages.ForgotPasswordValidationFailed);
                    return BadRequest(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.EmailRequired));
                }

                var success = await _userService.ForgotPasswordAsync(dto.Email);

                if (!success)
                {
                    _logger.LogWarning(ApplicationConstants.Messages.ForgotPasswordEmailNotFound, dto.Email);
                    return NotFound(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.EmailNotFound));
                }

                _logger.LogInformation(ApplicationConstants.Messages.ForgotPasswordCompleted, dto.Email);
                return Ok(ApiResponse<object>.SuccessResponse(new { }, ApplicationConstants.Messages.PasswordResetEmailSent));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.ForgotPasswordFailed);
                return BadRequest(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.ForgotPasswordFailed));
            }
        }

        [HttpPost(ApiConstants.Routes.TokenController.ResetPassword)]
        public async Task<ActionResult<ApiResponse<object>>> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingResetPassword, request?.Email ?? "unknown");

                if (string.IsNullOrEmpty(request?.Email) || string.IsNullOrEmpty(request?.ResetToken) || string.IsNullOrEmpty(request?.NewPassword))
                {
                    _logger.LogWarning(ApplicationConstants.Messages.ResetPasswordValidationFailed);
                    return BadRequest(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.ResetPasswordFieldsRequired));
                }

                var success = await _userService.ResetPasswordAsync(request.Email, request.ResetToken, request.NewPassword);

                if (!success)
                {
                    _logger.LogWarning(ApplicationConstants.Messages.ResetPasswordTokenInvalid, request.Email);
                    return BadRequest(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.InvalidOrExpiredResetToken));
                }

                _logger.LogInformation(ApplicationConstants.Messages.ResetPasswordCompleted, request.Email);
                return Ok(ApiResponse<object>.SuccessResponse(new { }, ApplicationConstants.Messages.PasswordResetSuccessfully));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, ApplicationConstants.ErrorMessages.ResetPasswordFailed);
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.ResetPasswordFailed);
                return BadRequest(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.ResetPasswordFailed));
            }
        }
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = null!;
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; } = null!;
        public string ResetToken { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
