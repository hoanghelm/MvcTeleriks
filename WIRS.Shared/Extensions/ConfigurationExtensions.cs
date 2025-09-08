using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WIRS.Shared.Configurations;
using WIRS.Shared.Helpers;

namespace WIRS.Shared.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddConfigurations(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<AppSettings>(
                configuration.GetSection("AppSettings"));

            services.Configure<AzureADSettings>(
                configuration.GetSection("AzureAD"));

            return services;
        }
    }

    public static class DBHelperExtensions
    {
        public static IServiceCollection AddDBHelpers(this IServiceCollection services)
        {
            services.AddScoped<IDBHelper, DBHelper>();

            return services;
        }
    }
}