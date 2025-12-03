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

                if (user.Status != Database.Enums.UserStatus.Active)
                    return Unauthorized(ApiResponse<object>.ErrorResponse("Account not verified"));

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

        [HttpPost("sendotp")]
        public async Task<ActionResult<ApiResponse<object>>> SendOTP([FromForm] SendOTPRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Purpose))
                    return BadRequest(ApiResponse<object>.ErrorResponse("Email and purpose are required"));

                // Validate purpose
                if (request.Purpose != "LOGIN" && request.Purpose != "REGISTER" && request.Purpose != "FORGETPASSWORD")
                    return BadRequest(ApiResponse<object>.ErrorResponse("Purpose must be LOGIN, REGISTER, or FORGETPASSWORD"));

                var user = await _userService.GetByEmailAsync(request.Email);
                
                // For REGISTER, user should not exist
                if (request.Purpose == "REGISTER" && user != null)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Email already exists"));
                
                // For LOGIN and FORGETPASSWORD, user must exist
                if ((request.Purpose == "LOGIN" || request.Purpose == "FORGETPASSWORD") && user == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("Email not found"));

                int userId = user?.UserId ?? 0;
                await _otpService.SendOTPAsync(userId, request.PhoneNumber ?? "", request.Email, request.Purpose);
                
                return Ok(ApiResponse<object>.SuccessResponse(new { 
                    message = "OTP sent successfully",
                    userId = userId,
                    purpose = request.Purpose
                }));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to send OTP"));
            }
        }

        [HttpPost("login/otp")]
        public async Task<ActionResult<ApiResponse<object>>> LoginWithOTP([FromForm] VerifyOTPRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.OTP))
                    return BadRequest(ApiResponse<object>.ErrorResponse("OTP is required"));

                var isValid = await _otpService.VerifyOTPAsync(request.UserId, request.OTP, "LOGIN");
                if (!isValid)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid or expired OTP"));

                var user = await _userService.GetByIdAsync(request.UserId);
                if (user == null || user.Status != Database.Enums.UserStatus.Active)
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

        [HttpPost("register/otp")]
        public async Task<ActionResult<ApiResponse<object>>> RegisterWithOTP([FromForm] RegisterOTPRequest request)
        {
            try
            {
                if (request == null || request.RegistrationData == null || string.IsNullOrEmpty(request.OTP))
                    return BadRequest(ApiResponse<object>.ErrorResponse("Registration data and OTP are required"));

                var isValid = await _otpService.VerifyOTPAsync(request.UserId, request.OTP, "REGISTER");
                if (!isValid)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid or expired OTP"));

                await _unitOfWork.BeginTransactionAsync();
                var userDto = await _userService.RegisterCustomerAsync(request.RegistrationData);
                await _userService.ActivateUserAsync(userDto.UserId);
                await _unitOfWork.CommitTransactionAsync();
                
                var user = await _userService.GetByIdAsync(userDto.UserId);
                var token = _tokenService.GenerateToken(user!);
                
                return Ok(ApiResponse<object>.SuccessResponse(new { 
                    message = "Registration completed successfully",
                    token,
                    username = user!.FullName,
                    role = user.Roles.ToString()
                }));
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return BadRequest(ApiResponse<object>.ErrorResponse("Registration failed"));
            }
        }



        [HttpPost("reset-password")]
        public async Task<ActionResult<ApiResponse<object>>> ResetPassword([FromForm] ResetPasswordOTPRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.OTP) || string.IsNullOrEmpty(request.NewPassword))
                    return BadRequest(ApiResponse<object>.ErrorResponse("OTP and new password are required"));

                var isValid = await _otpService.VerifyOTPAsync(request.UserId, request.OTP, "FORGETPASSWORD");
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
        public string Purpose { get; set; } = null!; // "LOGIN", "REGISTER", "FORGETPASSWORD"
    }

    public class RegisterOTPRequest
    {
        public int UserId { get; set; }
        public string OTP { get; set; } = null!;
        public CustomerRegistrationDTO RegistrationData { get; set; } = null!;
    }

    public class ResetPasswordOTPRequest
    {
        public int UserId { get; set; }
        public string OTP { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}