using Kanini.LMP.Api.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Application.Services.Implementations;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.EntitiesDtos;
using Kanini.LMP.Database.EntitiesDtos.Authentication;
using Kanini.LMP.Database.EntitiesDtos.Common;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost(ApiConstants.Routes.TokenController.Login)]
        public async Task<ActionResult<ApiResponse<object>>> Login([FromForm] LoginDTO loginDto)
        {
            try
            {
                Console.WriteLine($"Login attempt - Username: {loginDto?.Username}");
                
                if (loginDto == null || string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.PasswordHash))
                    return BadRequest(ApiResponse<object>.ErrorResponse("Username and password are required"));

                var user = await _userService.GetUserByEmailAsync(new StringDTO { Value = loginDto.Username });
                
                Console.WriteLine($"User found: {user != null}, Status: {user?.Status}");
                
                if (user == null)
                    return Unauthorized(ApiResponse<object>.ErrorResponse("User not found"));
            
                var passwordValid = PasswordService.VerifyPassword(loginDto.PasswordHash, user.PasswordHash);
                
                Console.WriteLine($"Password valid: {passwordValid}");
                
                if (!passwordValid)
                    return Unauthorized(ApiResponse<object>.ErrorResponse("Incorrect password"));

                if (user.Status == Database.Enums.UserStatus.Pending)
                {
                    await _otpService.GenerateOTPAsync(new StringDTO { Value = user.Email }, new StringDTO { Value = "REGISTER" });
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

        [HttpPost(ApiConstants.Routes.TokenController.SendOTP)]
        public async Task<ActionResult<ApiResponse<object>>> SendOTP([FromForm] SendOTPRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Email))
                    return BadRequest(ApiResponse<object>.ErrorResponse("Email is required"));

                var user = await _userService.GetUserByEmailAsync(new StringDTO { Value = request.Email });
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

                await _otpService.GenerateOTPAsync(new StringDTO { Value = request.Email }, new StringDTO { Value = purpose });
                
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

        [HttpPost(ApiConstants.Routes.TokenController.LoginWithOTP)]
        public async Task<ActionResult<ApiResponse<object>>> LoginWithOTP([FromForm] LoginOTPRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.OTP))
                    return BadRequest(ApiResponse<object>.ErrorResponse("Email and OTP are required"));

                var user = await _userService.GetUserByEmailAsync(new StringDTO { Value = request.Email });
                if (user == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("Email not found"));

                var isValid = await _otpService.VerifyOTPAsync(new StringDTO { Value = request.Email }, new StringDTO { Value = request.OTP }, new StringDTO { Value = "LOGIN" });
                if (!isValid.Value)
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

        [HttpPost(ApiConstants.Routes.TokenController.Register)]
        public async Task<ActionResult<ApiResponse<object>>> Register([FromForm] CustomerRegistrationDTO registrationData)
        {
            try
            {
                if (registrationData == null)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Registration data is required"));

                var existingUser = await _userService.GetUserByEmailAsync(new StringDTO { Value = registrationData.Email });
                if (existingUser != null && existingUser.Status == Database.Enums.UserStatus.Active)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Email already registered"));

                if (existingUser != null && existingUser.Status == Database.Enums.UserStatus.Pending)
                {
                    await _otpService.GenerateOTPAsync(new StringDTO { Value = registrationData.Email }, new StringDTO { Value = "REGISTER" });
                    return Ok(ApiResponse<object>.SuccessResponse(new { 
                        message = "Account exists but not verified. OTP sent to your email.",
                        userId = existingUser.UserId
                    }));
                }

                var userDto = await _userService.RegisterCustomerAsync(registrationData);
                await _otpService.GenerateOTPAsync(new StringDTO { Value = registrationData.Email }, new StringDTO { Value = "REGISTER" });
                
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

        [HttpPost(ApiConstants.Routes.TokenController.VerifyOTP)]
        public async Task<ActionResult<ApiResponse<object>>> VerifyOTP([FromForm] VerifyOTPRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.OTP))
                    return BadRequest(ApiResponse<object>.ErrorResponse("Email and OTP are required"));

                var user = await _userService.GetUserByEmailAsync(new StringDTO { Value = request.Email });
                if (user == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("User not found"));

                var isValid = await _otpService.VerifyOTPAsync(new StringDTO { Value = request.Email }, new StringDTO { Value = request.OTP }, new StringDTO { Value = request.Purpose });
                if (!isValid.Value)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid or expired OTP"));

                if (request.Purpose == "REGISTER")
                {
                    await _userService.ActivateUserAsync(new IdDTO { Id = user.UserId });
                    var token = _tokenService.GenerateToken(user);
                    return Ok(ApiResponse<object>.SuccessResponse(new
                    {
                        message = "Account verified successfully",
                        token,
                        username = user.FullName,
                        role = user.Roles.ToString()
                    }));
                }

                return Ok(ApiResponse<object>.SuccessResponse(new { message = "OTP verified successfully" }));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("OTP verification failed"));
            }
        }

        [HttpPost(ApiConstants.Routes.TokenController.ForgotPassword)]
        public async Task<ActionResult<ApiResponse<object>>> ForgotPassword([FromForm] ForgotPasswordRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Email))
                    return BadRequest(ApiResponse<object>.ErrorResponse("Email is required"));

                var user = await _userService.GetUserByEmailAsync(new StringDTO { Value = request.Email });
                if (user == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("Email not found"));

                await _otpService.GenerateOTPAsync(new StringDTO { Value = request.Email }, new StringDTO { Value = "RESET_PASSWORD" });
                
                return Ok(ApiResponse<object>.SuccessResponse(new { message = "Password reset OTP sent to your email" }));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to send password reset OTP"));
            }
        }

        [HttpPost(ApiConstants.Routes.TokenController.ResetPassword)]
        public async Task<ActionResult<ApiResponse<object>>> ResetPassword([FromForm] ResetPasswordRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.OTP) || string.IsNullOrEmpty(request.NewPassword))
                    return BadRequest(ApiResponse<object>.ErrorResponse("Email, OTP, and new password are required"));

                var user = await _userService.GetUserByEmailAsync(new StringDTO { Value = request.Email });
                if (user == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("User not found"));

                var isValid = await _otpService.VerifyOTPAsync(new StringDTO { Value = request.Email }, new StringDTO { Value = request.OTP }, new StringDTO { Value = "RESET_PASSWORD" });
                if (!isValid.Value)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid or expired OTP"));

                var hashedPassword = PasswordService.HashPassword(request.NewPassword);
                await _userService.ResetPasswordAsync(new StringDTO { Value = request.Email }, new StringDTO { Value = user.PasswordHash }, new StringDTO { Value = hashedPassword });

                return Ok(ApiResponse<object>.SuccessResponse(new { message = "Password reset successfully" }));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to reset password"));
            }
        }
    }
}