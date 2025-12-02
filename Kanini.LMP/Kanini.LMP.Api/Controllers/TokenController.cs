using Kanini.LMP.Api.Constants;
using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
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
        private readonly IOTPService _otpService;

        public TokenController(ITokenService tokenService, IUser userService, IUnitOfWork unitOfWork, IOTPService otpService)
        {
            _tokenService = tokenService;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _otpService = otpService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<object>>> Login([FromBody] LoginDTO loginDto)
        {
            try
            {
                if (loginDto == null || string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
                    return BadRequest(ApiResponse<object>.ErrorResponse("Email and password are required"));

                var token = await _tokenService.AuthenticateAsync(loginDto.Username, loginDto.Password);
                if (token == null)
                    return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid credentials"));

                var user = await _userService.GetByUsernameAsync(loginDto.Username);
                return Ok(ApiResponse<object>.SuccessResponse(new
                {
                    token,
                    username = user!.FullName,
                    role = user.Roles.ToString()
                }));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Login failed"));
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<object>>> RegisterCustomer([FromBody] CustomerRegistrationDTO registrationDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid registration data"));

                await _unitOfWork.BeginTransactionAsync();
                var userDto = await _userService.RegisterCustomerAsync(registrationDto!);
                await _otpService.SendOTPAsync(userDto.UserId, registrationDto.PhoneNumber, userDto.Email, "REGISTRATION");
                await _unitOfWork.CommitTransactionAsync();

                return Ok(ApiResponse<object>.SuccessResponse(new { 
                    message = "Registration successful. OTP sent for verification.",
                    userId = userDto.UserId
                }));
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return BadRequest(ApiResponse<object>.ErrorResponse("Registration failed"));
            }
        }

        [HttpPost("verify-otp")]
        public async Task<ActionResult<ApiResponse<object>>> VerifyOTP([FromBody] VerifyOTPRequest request)
        {
            try
            {
                var isValid = await _otpService.VerifyOTPAsync(request.UserId, request.OTP, "REGISTRATION");
                if (!isValid)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid or expired OTP"));

                return Ok(ApiResponse<object>.SuccessResponse(new { message = "Account verified successfully" }));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("OTP verification failed"));
            }
        }

        [HttpPost("login-otp")]
        public async Task<ActionResult<ApiResponse<object>>> LoginWithOTP([FromBody] OTPLoginRequest request)
        {
            try
            {
                var user = await _userService.GetByUsernameAsync(request.Email);
                if (user == null)
                    return Unauthorized(ApiResponse<object>.ErrorResponse("Email not found"));

                await _otpService.SendOTPAsync(user.UserId, "", user.Email, "LOGIN");
                return Ok(ApiResponse<object>.SuccessResponse(new { 
                    message = "OTP sent to your email",
                    userId = user.UserId
                }));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to send OTP"));
            }
        }

        [HttpPost("verify-login-otp")]
        public async Task<ActionResult<ApiResponse<object>>> VerifyLoginOTP([FromBody] VerifyOTPRequest request)
        {
            try
            {
                var isValid = await _otpService.VerifyOTPAsync(request.UserId, request.OTP, "LOGIN");
                if (!isValid)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid or expired OTP"));

                var user = await _userService.GetByIdAsync(request.UserId);
                var token = _tokenService.GenerateToken(user!);

                return Ok(ApiResponse<object>.SuccessResponse(new {
                    token,
                    username = user!.FullName,
                    role = user.Roles.ToString()
                }));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Login verification failed"));
            }
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<ApiResponse<object>>> ForgotPassword([FromBody] ForgotPasswordRequest dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto?.Email))
                    return BadRequest(ApiResponse<object>.ErrorResponse("Email is required"));

                var user = await _userService.GetByEmailAsync(dto.Email);
                if (user == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("Email not found"));

                await _otpService.SendOTPAsync(user.UserId, "", user.Email, "PASSWORD_RESET");
                return Ok(ApiResponse<object>.SuccessResponse(new { 
                    message = "OTP sent to your email",
                    userId = user.UserId
                }));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to send OTP"));
            }
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<ApiResponse<object>>> ResetPassword([FromBody] ResetPasswordOTPRequest request)
        {
            try
            {
                var isValid = await _otpService.VerifyOTPAsync(request.UserId, request.OTP, "PASSWORD_RESET");
                if (!isValid)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid or expired OTP"));

                var user = await _userService.GetByIdAsync(request.UserId);
                if (user == null)
                    return BadRequest(ApiResponse<object>.ErrorResponse("User not found"));

                var success = await _userService.ResetPasswordAsync(user.Email, "", request.NewPassword);
                if (!success)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Password reset failed"));

                return Ok(ApiResponse<object>.SuccessResponse(new { message = "Password reset successfully" }));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Password reset failed"));
            }
        }
    }

    public class VerifyOTPRequest
    {
        public int UserId { get; set; }
        public string OTP { get; set; } = null!;
    }

    public class OTPLoginRequest
    {
        public string Email { get; set; } = null!;
    }

    public class ResetPasswordOTPRequest
    {
        public int UserId { get; set; }
        public string OTP { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = null!;
    }
}