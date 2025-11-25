using Kanini.LMP.Data.Repositories.Implementations;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.EntitiesDtos.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.LMP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {

        private readonly ITokenService _tokenService;
        private readonly IUser _userService;

        public TokenController(IUser userService, ITokenService tokenService)
        {

            _tokenService = tokenService;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            try
            {
                if (loginDto == null || string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
                    return BadRequest(new { message = "Username and password are required" });

                var token = await _tokenService.AuthenticateAsync(loginDto.Username, loginDto.Password);

                if (token == null)
                    return Unauthorized(new { message = "Invalid credentials" });

                var user = await _userService.GetByUsernameAsync(loginDto.Username);

                return Ok(new
                {
                    token,
                    username = user.FullName,
                    role = user.Roles.ToString()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to process login", error = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterCustomer([FromBody] CustomerRegistrationDTO registrationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userDto = await _userService.RegisterCustomerAsync(registrationDto);

                return Ok(new
                {
                    message = "Customer registered successfully",
                    userId = userDto.UserId,
                    email = userDto.Email,
                    fullName = userDto.FullName
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Registration failed: {ex.Message}");
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest dto)
        {
            if (string.IsNullOrEmpty(dto.Email))
                return BadRequest("Email is required");

            var success = await _userService.ForgotPasswordAsync(dto.Email);

            if (!success)
                return NotFound("Email not found");

            return Ok(new { message = "Password reset email sent successfully" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.ResetToken) || string.IsNullOrEmpty(request.NewPassword))
                return BadRequest("Email, reset token, and new password are required");

            try
            {
                var success = await _userService.ResetPasswordAsync(request.Email, request.ResetToken, request.NewPassword);

                if (!success)
                    return BadRequest("Invalid or expired reset token");

                return Ok(new { message = "Password reset successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
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
