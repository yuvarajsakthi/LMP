using Kanini.LMP.Application.Mappings;
using Kanini.LMP.Application.Services.Implementations;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.Repositories.Implementations;
using Kanini.LMP.Data.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Kanini.LMP.Application.Extensions
{
    public static class ApplicationLayerExtensions
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            // AutoMapper
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // Memory Cache for credit score caching
            services.AddMemoryCache();

            // Service registrations
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUser, UserService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IEligibilityService, EligibilityService>();
            services.AddScoped<ILoanApplicationService, LoanApplicationService>();
            services.AddScoped<ILoanProductService, LoanProductService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IEmiCalculatorService, EmiCalculatorService>();
            services.AddScoped<IManagerWorkflowService, ManagerWorkflowService>();
            services.AddScoped<IManagerAnalyticsService, ManagerAnalyticsService>();
            services.AddHttpClient<IRazorpayService, RazorpayService>();
            services.AddScoped<IRazorpayService, RazorpayService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPdfService, PdfService>();
            services.AddScoped<IKYCService, KYCService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<ISMSService, SMSService>();
            services.AddScoped<IWhatsAppService, WhatsAppService>();
            services.AddScoped<IEnhancedNotificationService, EnhancedNotificationService>();
            services.AddHostedService<EMINotificationBackgroundService>();

            return services;
        }
    }
}