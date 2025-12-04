namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface IOTPService
    {
        Task<string> GenerateOTPAsync(string email, string purpose);
        Task<bool> VerifyOTPAsync(string email, string otp, string purpose);
        Task InvalidateOTPAsync(string email, string purpose);
    }
}