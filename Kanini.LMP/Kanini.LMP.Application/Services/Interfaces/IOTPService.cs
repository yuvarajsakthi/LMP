using Kanini.LMP.Database.EntitiesDtos.Common;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface IOTPService
    {
        Task<StringDTO> GenerateOTPAsync(StringDTO email, StringDTO purpose);
        Task<BoolDTO> VerifyOTPAsync(StringDTO email, StringDTO otp, StringDTO purpose);
        Task InvalidateOTPAsync(StringDTO email, StringDTO purpose);
    }
}