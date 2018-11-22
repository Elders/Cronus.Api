using Microsoft.Extensions.DependencyInjection;

namespace Elders.Cronus.Api
{
    public static class CronusApiExtensions
    {
        public static IServiceCollection AddCronusApi(this IServiceCollection services)
        {
            services.AddTransient<EventStoreExplorer>();
            services.AddTransient<ProjectionExplorer>();

            return services;
        }
    }
}
