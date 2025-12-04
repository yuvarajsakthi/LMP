using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Application.Services.Implementations;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.EntitiesDtos;
using Kanini.LMP.Database.EntitiesDtos.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.LMP.Api.Controllers
{
    [Route("auth")]
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
        public async Task<ActionResult<ApiResponse<object>>> Login([FromForm] LoginDTO loginDto)
        {
            try
            {
                if (loginDto == null || string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
                    return BadRequest(ApiResponse<object>.ErrorResponse("Username and password are required"));

                var user = await _userService.GetByUsernameAsync(loginDto.Username);
                if (user == null || !PasswordService.VerifyPassword(loginDto.Password, user.PasswordHash))
                    return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid credentials"));

                if (user.Status == Database.Enums.UserStatus.Pending)
                {
                    await _otpService.SendOTPAsync(user.UserId, "", user.Email, "REGISTER");
                    return Ok(ApiResponse<object>.SuccessResponse(new
                    {
                        requiresVerification = true,
                        message = "Account not verified. OTP sent to your email.",
                        email = user.Email
                    }));
                }

                if (user.Status != Database.Enums.UserStatus.Active)
                    return Unauthorized(ApiResponse<object>.ErrorResponse("Account is inactive"));

                var token = _tokenService.GenerateToken(user);
                return Ok(ApiResponse<object>.SuccessResponse(new
                {
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

        [HttpPost("sendotp/login")]
        public async Task<ActionResult<ApiResponse<object>>> SendLoginOTP([FromForm] SendOTPRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Email))
                    return BadRequest(ApiResponse<object>.ErrorResponse("Email is required"));

                var user = await _userService.GetByEmailAsync(request.Email);
                if (user == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("Email not found"));

                await _otpService.SendOTPAsync(user.UserId, request.PhoneNumber ?? "", request.Email, "LOGIN");
                
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

        [HttpPost("sendotp/register")]
        public async Task<ActionResult<ApiResponse<object>>> SendRegisterOTP([FromForm] SendOTPRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Email))
                    return BadRequest(ApiResponse<object>.ErrorResponse("Email is required"));

                var user = await _userService.GetByEmailAsync(request.Email);
                if (user != null && user.Status == Database.Enums.UserStatus.Active)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Email already exists"));

                await _otpService.SendOTPAsync(0, request.PhoneNumber ?? "", request.Email, "REGISTER");
                
                return Ok(ApiResponse<object>.SuccessResponse(new { 
                    message = "OTP sent successfully",
                    userId = 0
                }));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to send OTP"));
            }
        }

        [HttpPost("sendotp/forgetpassword")]
        public async Task<ActionResult<ApiResponse<object>>> SendForgetPasswordOTP([FromForm] SendOTPRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Email))
                    return BadRequest(ApiResponse<object>.ErrorResponse("Email is required"));

                var user = await _userService.GetByEmailAsync(request.Email);
                if (user == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("Email not found"));

                await _otpService.SendOTPAsync(user.UserId, request.PhoneNumber ?? "", request.Email, "FORGETPASSWORD");
                
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

        [HttpPost("login/otp")]
        public async Task<ActionResult<ApiResponse<object>>> LoginWithOTP([FromForm] LoginOTPRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.OTP))
                    return BadRequest(ApiResponse<object>.ErrorResponse("Email and OTP are required"));

                var user = await _userService.GetByEmailAsync(request.Email);
                if (user == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("Email not found"));

                var isValid = await _otpService.VerifyOTPAsync(user.UserId, request.OTP, "LOGIN");
                if (!isValid)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid or expired OTP"));

                if (user.Status != Database.Enums.UserStatus.Active)
                    return Unauthorized(ApiResponse<object>.ErrorResponse("Account not verified"));

                var token = _tokenService.GenerateToken(user);
                return Ok(ApiResponse<object>.SuccessResponse(new {
                    token,
                    username = user.FullName,
                    role = user.Roles.ToString()
                }));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Login verification failed"));
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<object>>> Register([FromForm] CustomerRegistrationDTO registrationData)
        {
            try
            {
                if (registrationData == null)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Registration data is required"));

                var existingUser = await _userService.GetByEmailAsync(registrationData.Email);
                if (existingUser != null && existingUser.Status == Database.Enums.UserStatus.Active)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Email already registered"));

                if (existingUser != null && existingUser.Status == Database.Enums.UserStatus.Pending)
                {
                    await _otpService.SendOTPAsync(existingUser.UserId, registrationData.PhoneNumber, registrationData.Email, "REGISTER");
                    return Ok(ApiResponse<object>.SuccessResponse(new { 
                        message = "Account exists but not verified. OTP sent to your email.",
                        userId = existingUser.UserId
                    }));
                }

                var userDto = await _userService.RegisterCustomerAsync(registrationData);
                await _otpService.SendOTPAsync(userDto.UserId, registrationData.PhoneNumber, registrationData.Email, "REGISTER");
                
                return Ok(ApiResponse<object>.SuccessResponse(new { 
                    message = "Registration successful. OTP sent to your email.",
                    userId = userDto.UserId
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse($"Registration failed: {ex.Message}"));
            }
        }

        [HttpPost("verify/otp")]
        public async Task<ActionResult<ApiResponse<object>>> VerifyOTP([FromForm] VerifyOTPRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.OTP))
                    return BadRequest(ApiResponse<object>.ErrorResponse("Email and OTP are required"));

                var user = await _userService.GetByEmailAsync(request.Email);
                if (user == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("User not found"));

                var isValid = await _otpService.VerifyOTPAsync(user.UserId, request.OTP, "REGISTER");
                if (!isValid)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid or expired OTP"));

                await _userService.ActivateUserAsync(user.UserId);
                
                return Ok(ApiResponse<object>.SuccessResponse(new { 
                    message = "Account verified successfully. You can now login."
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse($"Verification failed: {ex.Message}"));
            }
        }



        [HttpPost("reset-password")]
        public async Task<ActionResult<ApiResponse<object>>> ResetPassword([FromForm] ResetPasswordOTPRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.OTP) || string.IsNullOrEmpty(request.NewPassword))
                    return BadRequest(ApiResponse<object>.ErrorResponse("Email, OTP and new password are required"));
                    return BadRequest(ApiResponse<object>.ErrorResponse("Email, OTP and new password are required"));

                var user = await _userService.GetByEmailAsync(request.Email);
                if (user == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("Email not found"));

                var isValid = await _otpService.VerifyOTPAsync(user.UserId, request.OTP, "FORGETPASSWORD");
                if (!isValid)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid or expired OTP"));

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

    public class LoginOTPRequest
    {
        public string Email { get; set; } = null!;
        public string OTP { get; set; } = null!;
    }

    public class SendOTPRequest
    {
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
    }

    public class VerifyOTPRequest
    {
        public string Email { get; set; } = null!;
        public string OTP { get; set; } = null!;
    }

    public class ResetPasswordOTPRequest
    {
        public string Email { get; set; } = null!;
        public string OTP { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}