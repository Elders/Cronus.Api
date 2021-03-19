using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elders.Cronus.MessageProcessing;
using Elders.Cronus.Projections;
using Elders.Cronus.Projections.Versioning;

namespace Elders.Cronus.Api
{
    public class ProjectionExplorer
    {
        private readonly IProjectionReader projections;
        private readonly IProjectionStore projectionStore;
        private readonly CronusContext context;

        public ProjectionExplorer(IProjectionReader projections, IProjectionStore projectionStore, CronusContext context)
        {
            if (ReferenceEquals(null, projections) == true) throw new ArgumentNullException(nameof(projections));
            if (ReferenceEquals(null, projectionStore) == true) throw new ArgumentNullException(nameof(projectionStore));
            if (ReferenceEquals(null, context) == true) throw new ArgumentNullException(nameof(context));

            this.projections = projections;
            this.projectionStore = projectionStore;
            this.context = context;
        }

        public async Task<ProjectionDto> ExploreAsync(IBlobId id, Type projectionType)
        {
            var result = new ProjectionDto();
            var projectionResult = await projections.GetAsync(id, projectionType);
            if (projectionResult.IsSuccess)
            {
                result.Name = projectionType.Name;
                result.State = projectionResult.Data.State;
            }

            return result;
        }

        public async Task<ProjectionDto> ExploreIncludingEventsAsync(IBlobId id, Type projectionType)
        {
            ProjectionDto result = await ExploreAsync(id, projectionType).ConfigureAwait(false);

            bool projectionHasState = result.State is null == false;
            if (projectionHasState)
            {
                ProjectionVersion liveVersion = await GetLiveVersion(projectionType);

                if (liveVersion is null == false)
                {
                    IEnumerable<ProjectionCommit> projectionCommits = projectionStore.EnumerateProjection(liveVersion, id);
                    result.Events = projectionCommits.ExtractEventData();
                }
            }

            return result;
        }

        private async Task<ProjectionVersion> GetLiveVersion(Type projectionType)
        {
            var projectionVersionManagerId = new ProjectionVersionManagerId(projectionType.GetContractId(), context.Tenant);
            ProjectionDto dto = await ExploreAsync(projectionVersionManagerId, typeof(ProjectionVersionsHandler));
            var state = dto?.State as ProjectionVersionsHandlerState;

            ProjectionVersion liveVersion = state is null ? null : state.AllVersions.GetLive();
            return liveVersion;
        }

        public class ProjectionDto
        {
            public string Name { get; set; }
            public object State { get; set; }
            public IEnumerable<ProjectionEventDto> Events { get; set; }
        }

        public class ProjectionEventDto
        {
            public object Data { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}
