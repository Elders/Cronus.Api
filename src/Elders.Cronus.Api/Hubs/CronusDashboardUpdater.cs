using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Elders.Cronus.Discoveries;
using Elders.Cronus.Projections;
using System.Threading.Tasks;

namespace Elders.Cronus.Api.Hubs
{
    public class CronusDashboardUpdater : ISystemTrigger,
        ISignalHandle<RebuildProjectionProgress>,
        ISignalHandle<RebuildProjectionStarted>,
        ISignalHandle<RebuildProjectionFinished>
    {
        private readonly IHubContext<RebuildProjectionHub> hub;

        public CronusDashboardUpdater(ICronusApiAccessor cronusApiAccessor)
        {
            if (cronusApiAccessor?.Provider is null == false)
                this.hub = cronusApiAccessor.Provider.GetRequiredService<IHubContext<RebuildProjectionHub>>();
        }

        public async Task HandleAsync(RebuildProjectionProgress signal)
        {
            await hub.ReportProgressAsync(signal.ProjectionTypeId, signal.ProcessedCount, signal.TotalCount).ConfigureAwait(false);
        }

        public async Task HandleAsync(RebuildProjectionStarted signal)
        {
            await hub.RebuildStartedAsync(signal.ProjectionTypeId).ConfigureAwait(false);
        }

        public async Task HandleAsync(RebuildProjectionFinished signal)
        {
            await hub.RebuildFinishedAsync(signal.ProjectionTypeId).ConfigureAwait(false);
        }
    }
}
