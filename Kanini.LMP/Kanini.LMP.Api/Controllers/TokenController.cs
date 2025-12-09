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
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.PasswordHash))
                throw new ArgumentException("Username and password are required");

            var user = await _userService.GetUserByEmailAsync(new StringDTO { Value = loginDto.Username });
            
            if (user == null)
                throw new KeyNotFoundException("User not found");
        
            var passwordValid = PasswordService.VerifyPassword(loginDto.PasswordHash, user.PasswordHash);
            
            if (!passwordValid)
                throw new UnauthorizedAccessException("Incorrect password");

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
                throw new UnauthorizedAccessException("Account is inactive");

            var token = _tokenService.GenerateToken(user);
            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                token,
                username = user.FullName,
                role = user.Roles.ToString()
            }));
        }

        [HttpPost(ApiConstants.Routes.TokenController.SendOTP)]
        public async Task<ActionResult<ApiResponse<object>>> SendOTP([FromForm] SendOTPRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email))
                throw new ArgumentException("Email is required");

            var user = await _userService.GetUserByEmailAsync(new StringDTO { Value = request.Email });
            var purpose = request.Purpose.ToString();

            if (request.Purpose == OTPPurpose.REGISTER)
            {
                if (user != null && user.Status == Database.Enums.UserStatus.Active)
                    throw new InvalidOperationException("Email already exists");
            }
            else
            {
                if (user == null)
                    throw new KeyNotFoundException("Email not found");
            }

            await _otpService.GenerateOTPAsync(new StringDTO { Value = request.Email }, new StringDTO { Value = purpose });
            
            return Ok(ApiResponse<object>.SuccessResponse(new { 
                message = "OTP sent successfully",
                userId = user?.UserId ?? 0
            }));
        }

        [HttpPost(ApiConstants.Routes.TokenController.LoginWithOTP)]
        public async Task<ActionResult<ApiResponse<object>>> LoginWithOTP([FromForm] LoginOTPRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.OTP))
                throw new ArgumentException("Email and OTP are required");

            var user = await _userService.GetUserByEmailAsync(new StringDTO { Value = request.Email });
            if (user == null)
                throw new KeyNotFoundException("Email not found");

            var isValid = await _otpService.VerifyOTPAsync(new StringDTO { Value = request.Email }, new StringDTO { Value = request.OTP }, new StringDTO { Value = "LOGIN" });
            if (!isValid.Value)
                throw new InvalidOperationException("Invalid or expired OTP");

            if (user.Status != Database.Enums.UserStatus.Active)
                throw new UnauthorizedAccessException("Account not verified");

            var token = _tokenService.GenerateToken(user);
            return Ok(ApiResponse<object>.SuccessResponse(new {
                token,
                username = user.FullName,
                role = user.Roles.ToString()
            }));
        }

        [HttpPost(ApiConstants.Routes.TokenController.Register)]
        public async Task<ActionResult<ApiResponse<object>>> Register([FromForm] CustomerRegistrationDTO registrationData)
        {
            if (registrationData == null)
                throw new ArgumentNullException(nameof(registrationData), "Registration data is required");

            var existingUser = await _userService.GetUserByEmailAsync(new StringDTO { Value = registrationData.Email });
            if (existingUser != null && existingUser.Status == Database.Enums.UserStatus.Active)
                throw new InvalidOperationException("Email already registered");

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

        [HttpPost(ApiConstants.Routes.TokenController.VerifyOTP)]
        public async Task<ActionResult<ApiResponse<object>>> VerifyOTP([FromForm] VerifyOTPRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email))
                throw new ArgumentException("Email and OTP are required");

            var user = await _userService.GetUserByEmailAsync(new StringDTO { Value = request.Email });
            if (user == null)
                throw new KeyNotFoundException("User not found");

            var isValid = await _otpService.VerifyOTPAsync(new StringDTO { Value = request.Email }, new StringDTO { Value = request.OTP }, new StringDTO { Value = request.Purpose });
            if (!isValid.Value)
                throw new InvalidOperationException("Invalid or expired OTP");

            if (request.Purpose == "REGISTER")
            {
                user.Status = Database.Enums.UserStatus.Active;
                await _unitOfWork.SaveChangesAsync();
            }

            var token = _tokenService.GenerateToken(user);
            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                token,
                username = user.FullName,
                role = user.Roles.ToString(),
                message = "Verification successful"
            }));
        }

        [HttpPost(ApiConstants.Routes.TokenController.ForgotPassword)]
        public async Task<ActionResult<ApiResponse<object>>> ForgotPassword([FromForm] ForgotPasswordRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email))
                throw new ArgumentException("Email is required");

            var user = await _userService.GetUserByEmailAsync(new StringDTO { Value = request.Email });
            if (user == null)
                throw new KeyNotFoundException("Email not found");

            await _otpService.GenerateOTPAsync(new StringDTO { Value = request.Email }, new StringDTO { Value = "RESET_PASSWORD" });
            
            return Ok(ApiResponse<object>.SuccessResponse(new { message = "Password reset OTP sent to your email" }));
        }

        [HttpPost(ApiConstants.Routes.TokenController.ResetPassword)]
        public async Task<ActionResult<ApiResponse<object>>> ResetPassword([FromForm] ResetPasswordRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.OTP) || string.IsNullOrEmpty(request.NewPassword))
                throw new ArgumentException("Email, OTP, and new password are required");

            var user = await _userService.GetUserByEmailAsync(new StringDTO { Value = request.Email });
            if (user == null)
                throw new KeyNotFoundException("User not found");

            var isValid = await _otpService.VerifyOTPAsync(new StringDTO { Value = request.Email }, new StringDTO { Value = request.OTP }, new StringDTO { Value = "RESET_PASSWORD" });
            if (!isValid.Value)
                throw new InvalidOperationException("Invalid or expired OTP");

            var hashedPassword = PasswordService.HashPassword(request.NewPassword);
            await _userService.ResetPasswordAsync(new StringDTO { Value = request.Email }, new StringDTO { Value = user.PasswordHash }, new StringDTO { Value = hashedPassword });

            return Ok(ApiResponse<object>.SuccessResponse(new { message = "Password reset successfully" }));
        }
    }
}
