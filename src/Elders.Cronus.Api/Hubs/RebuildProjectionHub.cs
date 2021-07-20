using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Elders.Cronus.Api.Hubs
{
    public class RebuildProjectionHub : Hub { }

    public static class RebuildProjectionHubExtensions
    {
        public static Task ReportProgress(this IHubContext<RebuildProjectionHub> hub, string projectionTypeId, long processedCount, long totalCount)
        {
            if (hub?.Clients?.All is null == false)
            {
                hub.Clients.All.SendCoreAsync("RebuildProgress", new object[] { projectionTypeId, processedCount, totalCount }).GetAwaiter().GetResult();
            }

            return Task.CompletedTask;
        }

        public static Task RebuildStarted(this IHubContext<RebuildProjectionHub> hub, string projectionTypeId)
        {
            if (hub?.Clients?.All is null == false)
            {
                hub.Clients.All.SendCoreAsync("RebuildStarted", new object[] { projectionTypeId }).GetAwaiter().GetResult();
            }

            return Task.CompletedTask;
        }

        public static Task RebuildFinished(this IHubContext<RebuildProjectionHub> hub, string projectionTypeId)
        {
            if (hub?.Clients?.All is null == false)
            {
                hub.Clients.All.SendCoreAsync("RebuildFinished", new object[] { projectionTypeId }).GetAwaiter().GetResult();
            }

            return Task.CompletedTask;
        }
    }
}
