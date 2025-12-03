using Kanini.LMP.Api.Constants;
using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Application.Services.Implementations;
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

                // Check if user exists and get user details first
                var user = await _userService.GetByUsernameAsync(loginDto.Username);
                if (user == null)
                    return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid credentials"));

                // Verify password first
                if (!PasswordService.VerifyPassword(loginDto.Password, user.PasswordHash))
                    return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid credentials"));

                // If account is not verified, send OTP for verification
                if (user.Status != Database.Enums.UserStatus.Active)
                {
                    await _otpService.SendOTPAsync(user.UserId, "", user.Email, "REGISTRATION");
                    return Ok(ApiResponse<object>.SuccessResponse(new
                    {
                        requiresVerification = true,
                        message = "Account not verified. OTP sent to your email for verification.",
                        userId = user.UserId
                    }));
                }

                // Generate token for verified users
                var token = _tokenService.GenerateToken(user);
                return Ok(ApiResponse<object>.SuccessResponse(new
                {
                    requiresVerification = false,
                    token,
                    username = user.FullName,
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

                await _userService.ActivateUserAsync(request.UserId);
                
                // Get user details and generate token for immediate login
                var user = await _userService.GetByIdAsync(request.UserId);
                var token = _tokenService.GenerateToken(user!);
                
                return Ok(ApiResponse<object>.SuccessResponse(new { 
                    message = "Account verified successfully",
                    token,
                    username = user!.FullName,
                    role = user.Roles.ToString()
                }));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("OTP verification failed"));
            }
        }

        [HttpPost("send-otp")]
        public async Task<ActionResult<ApiResponse<object>>> SendOTP([FromBody] SendOTPRequest request)
        {
            try
            {
                var user = await _userService.GetByEmailAsync(request.Email);
                if (user == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("Email not found"));

                await _otpService.SendOTPAsync(user.UserId, request.PhoneNumber ?? "", user.Email, request.Purpose);
                return Ok(ApiResponse<object>.SuccessResponse(new { 
                    message = "OTP sent successfully",
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
                if (user!.Status != Database.Enums.UserStatus.Active)
                    return Unauthorized(ApiResponse<object>.ErrorResponse("Account not verified. Please verify your email first."));

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



        [HttpPost("resend-verification")]
        public async Task<ActionResult<ApiResponse<object>>> ResendVerification([FromBody] ResendVerificationRequest request)
        {
            try
            {
                var user = await _userService.GetByEmailAsync(request.Email);
                if (user == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("Email not found"));

                if (user.Status == Database.Enums.UserStatus.Active)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Account is already verified"));

                await _otpService.SendOTPAsync(user.UserId, "", user.Email, "REGISTRATION");
                return Ok(ApiResponse<object>.SuccessResponse(new { 
                    message = "Verification OTP sent successfully",
                    userId = user.UserId
                }));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to send verification OTP"));
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

    public class SendOTPRequest
    {
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string Purpose { get; set; } = null!; // "LOGIN", "REGISTRATION", "PASSWORD_RESET"
    }

    public class ResendVerificationRequest
    {
        public string Email { get; set; } = null!;
    }

    public class ResetPasswordOTPRequest
    {
        public int UserId { get; set; }
        public string OTP { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}