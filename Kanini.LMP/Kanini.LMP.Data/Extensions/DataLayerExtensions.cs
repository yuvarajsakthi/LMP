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
            services.AddScoped<IManagerAnalyticsRepository, ManagerAnalyticsRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

            return services;
        }
    }
}