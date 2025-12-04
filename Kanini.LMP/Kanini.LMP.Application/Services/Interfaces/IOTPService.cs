namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface IOTPService
    {
        Task<string> GenerateOTPAsync(int userId, string purpose);
        Task<string> GenerateOTPAsync(string email, string purpose);
        Task<bool> VerifyOTPAsync(int userId, string otp, string purpose);
        Task<bool> VerifyOTPAsync(string email, string otp, string purpose);
        Task<bool> SendOTPAsync(int userId, string phoneNumber, string email, string purpose);
        Task InvalidateOTPAsync(int userId, string purpose);
    }
}