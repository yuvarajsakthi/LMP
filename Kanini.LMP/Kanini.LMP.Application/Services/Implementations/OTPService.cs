using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.Email;
using Kanini.LMP.Database.EntitiesDtos.Common;
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

        public async Task<StringDTO> GenerateOTPAsync(StringDTO email, StringDTO purpose)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            var cacheKey = $"OTP_{email.Value}_{purpose.Value}";
            
            _cache.Set(cacheKey, otp, TimeSpan.FromMinutes(5));
            
            _logger.LogInformation($"OTP generated for email {email.Value}, purpose: {purpose.Value}, OTP: {otp}");
            
            try
            {
                await _emailService.SendOTPEmailAsync(new OTPEmailDto
                {
                    Email = email.Value,
                    Name = email.Value.Split('@')[0],
                    OTP = otp,
                    Purpose = purpose.Value
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failed to send email, but OTP is cached. OTP: {otp}, Error: {ex.Message}");
            }
            return new StringDTO { Value = otp };
        }

        public async Task<BoolDTO> VerifyOTPAsync(StringDTO email, StringDTO otp, StringDTO purpose)
        {
            var cacheKey = $"OTP_{email.Value}_{purpose.Value}";
            
            if (_cache.TryGetValue(cacheKey, out string? cachedOtp) && cachedOtp == otp.Value)
            {
                _cache.Remove(cacheKey);
                _logger.LogInformation($"OTP verified successfully for email {email.Value}, purpose: {purpose.Value}");
                return new BoolDTO { Value = true };
            }
            
            _logger.LogWarning($"OTP verification failed for email {email.Value}, purpose: {purpose.Value}");
            return new BoolDTO { Value = false };
        }

        public async Task InvalidateOTPAsync(StringDTO email, StringDTO purpose)
        {
            var cacheKey = $"OTP_{email.Value}_{purpose.Value}";
            _cache.Remove(cacheKey);
            _logger.LogInformation($"OTP invalidated for email {email.Value}, purpose: {purpose.Value}");
        }
    }
}