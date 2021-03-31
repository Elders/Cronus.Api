using Elders.Cronus.Discoveries;
using Elders.Cronus.Projections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

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

        public void Handle(RebuildProjectionProgress signal)
        {
            hub.ReportProgress(signal.ProjectionTypeId, signal.ProcessedCount, signal.TotalCount).GetAwaiter().GetResult();
        }

        public void Handle(RebuildProjectionStarted signal)
        {
            hub.RebuildStarted(signal.ProjectionTypeId).GetAwaiter().GetResult();
        }

        public void Handle(RebuildProjectionFinished signal)
        {
            hub.RebuildFinished(signal.ProjectionTypeId).GetAwaiter().GetResult();
        }
    }
}
