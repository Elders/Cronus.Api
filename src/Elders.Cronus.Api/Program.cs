using Elders.Cronus.Discoveries;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Elders.Cronus.Api
{
    public class Program
    {
        public static void Main(string[] args) { }
    }

    public class CronusApiDiscovery : DiscoveryBasedOnExecutingDirAssemblies<IConsumer<object>>
    {
        protected override DiscoveryResult<IConsumer<object>> DiscoverFromAssemblies(DiscoveryContext context)
        {
            return new DiscoveryResult<IConsumer<object>>(GetModels());
        }

        IEnumerable<DiscoveredModel> GetModels()
        {
            yield return new DiscoveredModel(typeof(IConsumer<>), typeof(CronusStart), ServiceLifetime.Singleton);
        }
    }
}
