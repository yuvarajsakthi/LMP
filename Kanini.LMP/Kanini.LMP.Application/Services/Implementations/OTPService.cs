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
        private readonly IEmailService _emailService;
        private readonly IWhatsAppService _whatsAppService;
        private readonly ILogger<OTPService> _logger;

        public OTPService(IMemoryCache cache, IEmailService emailService, IWhatsAppService whatsAppService, ILogger<OTPService> logger)
        {
            _cache = cache;
            _emailService = emailService;
            _whatsAppService = whatsAppService;
            _logger = logger;
        }

        public async Task<string> GenerateOTPAsync(int userId, string purpose)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            var cacheKey = $"OTP_{userId}_{purpose}";
            
            _cache.Set(cacheKey, otp, TimeSpan.FromMinutes(5));
            
            _logger.LogInformation($"OTP generated for user {userId}, purpose: {purpose}");
            return otp;
        }

        public async Task<bool> VerifyOTPAsync(int userId, string otp, string purpose)
        {
            var cacheKey = $"OTP_{userId}_{purpose}";
            
            if (_cache.TryGetValue(cacheKey, out string? cachedOtp) && cachedOtp == otp)
            {
                _cache.Remove(cacheKey);
                _logger.LogInformation($"OTP verified successfully for user {userId}, purpose: {purpose}");
                return true;
            }
            
            _logger.LogWarning($"OTP verification failed for user {userId}, purpose: {purpose}");
            return false;
        }

        public async Task<bool> SendOTPAsync(int userId, string phoneNumber, string email, string purpose)
        {
            try
            {
                var otp = await GenerateOTPAsync(userId, purpose);
                
                var message = purpose switch
                {
                    "REGISTRATION" => $"Your registration OTP: {otp}. Valid for 5 minutes.",
                    "LOGIN" => $"Your login OTP: {otp}. Valid for 5 minutes.",
                    "PASSWORD_RESET" => $"Your password reset OTP: {otp}. Valid for 5 minutes.",
                    _ => $"Your OTP: {otp}. Valid for 5 minutes."
                };

                // Send via WhatsApp
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    await _whatsAppService.SendWhatsAppMessageAsync(phoneNumber, message);
                }

                // Send via Email as backup
                if (!string.IsNullOrEmpty(email))
                {
                    await _emailService.SendEmailAsync(new Database.EntitiesDto.Email.EmailDto
                    {
                        ToEmail = email,
                        Subject = "OTP Verification",
                        Body = message
                    });
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send OTP for user {userId}, purpose: {purpose}");
                return false;
            }
        }

        public async Task InvalidateOTPAsync(int userId, string purpose)
        {
            var cacheKey = $"OTP_{userId}_{purpose}";
            _cache.Remove(cacheKey);
            _logger.LogInformation($"OTP invalidated for user {userId}, purpose: {purpose}");
        }
    }
}