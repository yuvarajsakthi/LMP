using Kanini.LMP.Data.Data;
using Kanini.LMP.Data.Repositories.Implementations;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kanini.LMP.Data.Extensions
{
    public static class DataLayerExtensions
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<LmpDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("LMPConnect"),
                    b => b.MigrationsAssembly("Kanini.LMP.Data")
                ));

            // Repository registrations
            services.AddScoped(typeof(ILMPRepository<,>), typeof(LMPRepositoy<,>));
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IApplicationDocumentLinkRepository, ApplicationDocumentLinkRepository>();
            services.AddScoped<IEMIRepository, EMIRepository>();
            services.AddScoped<IManagerAnalyticsRepository, ManagerAnalyticsRepository>();
            services.AddScoped<IPdfRepository, PdfRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

            return services;
        }
    }
}