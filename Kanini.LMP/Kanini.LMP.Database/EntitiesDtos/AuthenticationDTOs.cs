using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDtos.Authentication
{
    public class LoginDTO
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;
    }

    public class LoginOTPRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string OTP { get; set; } = null!;
    }

    public class SendOTPRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public OTPPurpose Purpose { get; set; }
    }

    public class VerifyOTPRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string OTP { get; set; } = null!;

        [Required]
        public string Purpose { get; set; } = null!;
    }

    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }

    public class ResetPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string OTP { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = null!;
    }

    public enum OTPPurpose
    {
        REGISTER,
        LOGIN,
        RESET_PASSWORD
    }
}
