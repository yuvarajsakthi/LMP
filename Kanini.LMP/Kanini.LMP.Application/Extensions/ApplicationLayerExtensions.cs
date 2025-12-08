using Kanini.LMP.Application.Mappings;
using Kanini.LMP.Application.Services.Implementations;
using Kanini.LMP.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Kanini.LMP.Application.Extensions
{
    public static class ApplicationLayerExtensions
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            // AutoMapper
            services.AddAutoMapper(typeof(UserProfile), typeof(CustomerProfile), typeof(LoanProductProfile), typeof(NotificationProfile), typeof(FaqProfile), typeof(LoanApplicationDTOProfile));

            // Memory Cache for credit score caching
            services.AddMemoryCache();

            // Service registrations
            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IEligibilityService, EligibilityService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEmiCalculatorService, EmiCalculatorService>();
            services.AddScoped<ILoanApplicationService, LoanApplicationService>();
            services.AddScoped<ILoanProductService, LoanProductService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IOTPService, OTPService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IFaqService, FaqService>();
            services.AddScoped<IPdfService, PdfService>();
            services.AddScoped<IManagerDashboardService, ManagerDashboardService>();
            
            return services;
        }
    }
}