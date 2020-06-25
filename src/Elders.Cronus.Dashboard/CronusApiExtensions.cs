using Elders.Cronus.Dashboard.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Elders.Cronus.Dashboard
{
    public static class CronusApiExtensions
    {
        public static IServiceCollection AddCronusApi(this IServiceCollection services)
        {
            services.AddSingleton<EventStoreExplorer>();
            services.AddSingleton<CronusClient>();

            return services;
        }
    }
}
