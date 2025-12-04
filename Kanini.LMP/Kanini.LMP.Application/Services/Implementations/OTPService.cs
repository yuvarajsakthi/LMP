using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class OTPService : IOTPService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<OTPService> _logger;
        private readonly IEmailService _emailService;

        public OTPService(IMemoryCache cache, ILogger<OTPService> logger, IEmailService emailService)
        {
            _cache = cache;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<string> GenerateOTPAsync(string email, string purpose)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            var cacheKey = $"OTP_{email}_{purpose}";
            
            _cache.Set(cacheKey, otp, TimeSpan.FromMinutes(5));
            
            await _emailService.SendOTPEmailAsync(email, email.Split('@')[0], otp, purpose);
            
            _logger.LogInformation($"OTP generated for email {email}, purpose: {purpose}");
            return otp;
        }

        public async Task<bool> VerifyOTPAsync(string email, string otp, string purpose)
        {
            var cacheKey = $"OTP_{email}_{purpose}";
            
            if (_cache.TryGetValue(cacheKey, out string? cachedOtp) && cachedOtp == otp)
            {
                _cache.Remove(cacheKey);
                _logger.LogInformation($"OTP verified successfully for email {email}, purpose: {purpose}");
                return true;
            }
            
            _logger.LogWarning($"OTP verification failed for email {email}, purpose: {purpose}");
            return false;
        }

        public async Task InvalidateOTPAsync(string email, string purpose)
        {
            var cacheKey = $"OTP_{email}_{purpose}";
            _cache.Remove(cacheKey);
            _logger.LogInformation($"OTP invalidated for email {email}, purpose: {purpose}");
        }
    }
}