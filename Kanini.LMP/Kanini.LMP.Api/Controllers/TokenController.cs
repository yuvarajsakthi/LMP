using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Application.Services.Implementations;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.EntitiesDtos;
using Kanini.LMP.Database.EntitiesDtos.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Kanini.LMP.Api.Controllers
{
    [Route("auth")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOTPService _otpService;

        public TokenController(ITokenService tokenService, IUserService userService, IUnitOfWork unitOfWork, IOTPService otpService)
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
                Console.WriteLine("Login endpoint hit");
                Console.WriteLine($"loginDto is null: {loginDto == null}");
                if (loginDto != null)
                {
                    Console.WriteLine($"Username: {loginDto.Username}");
                    Console.WriteLine($"Password: {loginDto.PasswordHash}");
                }
                if (loginDto == null || string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.PasswordHash))
                    return BadRequest(ApiResponse<object>.ErrorResponse("Username and password are required"));

                var user = await _userService.GetUserByEmailAsync(loginDto.Username);
                Console.WriteLine($"User found: {user != null}");
                
                if (user == null)
                    return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid credentials"));
            
                
                var passwordValid = PasswordService.VerifyPassword(loginDto.PasswordHash, user.PasswordHash);
                Console.WriteLine($"Password valid: {passwordValid}");
                
                if (!passwordValid)
                    return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid credentials"));

                if (user.Status == Database.Enums.UserStatus.Pending)
                {
                    await _otpService.GenerateOTPAsync(user.Email, "REGISTER");
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
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return BadRequest(ApiResponse<object>.ErrorResponse("Login failed"));
            }
        }

        [HttpPost("sendotp")]
        public async Task<ActionResult<ApiResponse<object>>> SendOTP([FromForm] SendOTPRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Email))
                    return BadRequest(ApiResponse<object>.ErrorResponse("Email is required"));

                var user = await _userService.GetUserByEmailAsync(request.Email);
                var purpose = request.Purpose.ToString();

                if (request.Purpose == OTPPurpose.REGISTER)
                {
                    if (user != null && user.Status == Database.Enums.UserStatus.Active)
                        return BadRequest(ApiResponse<object>.ErrorResponse("Email already exists"));
                }
                else
                {
                    if (user == null)
                        return NotFound(ApiResponse<object>.ErrorResponse("Email not found"));
                }

                await _otpService.GenerateOTPAsync(request.Email, purpose);
                
                return Ok(ApiResponse<object>.SuccessResponse(new { 
                    message = "OTP sent successfully",
                    userId = user?.UserId ?? 0
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

                var user = await _userService.GetUserByEmailAsync(request.Email);
                if (user == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("Email not found"));

                var isValid = await _otpService.VerifyOTPAsync(request.Email, request.OTP, "LOGIN");
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

                var existingUser = await _userService.GetUserByEmailAsync(registrationData.Email);
                if (existingUser != null && existingUser.Status == Database.Enums.UserStatus.Active)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Email already registered"));

                if (existingUser != null && existingUser.Status == Database.Enums.UserStatus.Pending)
                {
                    await _otpService.GenerateOTPAsync(registrationData.Email, "REGISTER");
                    return Ok(ApiResponse<object>.SuccessResponse(new { 
                        message = "Account exists but not verified. OTP sent to your email.",
                        userId = existingUser.UserId
                    }));
                }

                var userDto = await _userService.RegisterCustomerAsync(registrationData);
                await _otpService.GenerateOTPAsync(registrationData.Email, "REGISTER");
                
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

                var user = await _userService.GetUserByEmailAsync(request.Email);
                if (user == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("User not found"));

                var isValid = await _otpService.VerifyOTPAsync(request.Email, request.OTP, "REGISTER");
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

                var user = await _userService.GetUserByEmailAsync(request.Email);
                if (user == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("Email not found"));

                var isValid = await _otpService.VerifyOTPAsync(request.Email, request.OTP, "FORGETPASSWORD");
                if (!isValid)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid or expired OTP"));

                var success = await _userService.ResetPasswordAsync(request.Email, "", request.NewPassword);
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
        public OTPPurpose Purpose { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OTPPurpose
    {
        LOGIN,
        REGISTER,
        FORGETPASSWORD
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