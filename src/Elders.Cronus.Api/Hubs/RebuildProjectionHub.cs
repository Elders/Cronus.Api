using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Elders.Cronus.Api.Hubs
{
    public class RebuildProjectionHub : Hub { }

    public static class RebuildProjectionHubExtensions
    {
        public static async Task ReportProgressAsync(this IHubContext<RebuildProjectionHub> hub, string projectionTypeId, long processedCount, long totalCount)
        {
            if (hub?.Clients?.All is null == false)
            {
                await hub.Clients.All.SendCoreAsync("RebuildProgress", new object[] { projectionTypeId, processedCount, totalCount });
            }
        }

        public static async Task RebuildStartedAsync(this IHubContext<RebuildProjectionHub> hub, string projectionTypeId)
        {
            if (hub?.Clients?.All is null == false)
            {
                await hub.Clients.All.SendCoreAsync("RebuildStarted", new object[] { projectionTypeId }).ConfigureAwait(false);
            }
        }

        public static async Task RebuildFinishedAsync(this IHubContext<RebuildProjectionHub> hub, string projectionTypeId)
        {
            if (hub?.Clients?.All is null == false)
            {
                await hub.Clients.All.SendCoreAsync("RebuildFinished", new object[] { projectionTypeId }).ConfigureAwait(false);
            }
        }
    }
}
