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
        private readonly ICronusContextAccessor contextAccessor;

        public ProjectionExplorer(IProjectionReader projections, IProjectionStore projectionStore, ICronusContextAccessor contextAccessor)
        {
            if (ReferenceEquals(null, projections) == true) throw new ArgumentNullException(nameof(projections));
            if (ReferenceEquals(null, projectionStore) == true) throw new ArgumentNullException(nameof(projectionStore));
            if (ReferenceEquals(null, contextAccessor) == true) throw new ArgumentNullException(nameof(contextAccessor));

            this.projections = projections;
            this.projectionStore = projectionStore;
            this.contextAccessor = contextAccessor;
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
                    var projectionCommits = projectionStore.EnumerateProjectionAsync(liveVersion, id).ConfigureAwait(false);

                    await foreach (var commit in projectionCommits)
                    {
                        result.Commits.Add(commit.ToProjectionDto());
                    }
                }
            }

            return result;
        }

        private async Task<ProjectionVersion> GetLiveVersion(Type projectionType)
        {
            var projectionVersionManagerId = new ProjectionVersionManagerId(projectionType.GetContractId(), contextAccessor.CronusContext.Tenant);
            ProjectionDto dto = await ExploreAsync(projectionVersionManagerId, typeof(ProjectionVersionsHandler)).ConfigureAwait(false);
            var state = dto?.State as ProjectionVersionsHandlerState;

            ProjectionVersion liveVersion = state is null ? null : state.AllVersions.GetLive();
            return liveVersion;
        }
    }

    public class ProjectionDto
    {
        public ProjectionDto()
        {
            Commits = new List<ProjectionCommitDto>();
        }

        public string Name { get; set; }
        public object State { get; set; }
        public List<ProjectionCommitDto> Commits { get; set; }
    }

    public class ProjectionCommitDto
    {
        public ProjectionCommitDto()
        {
            Events = new List<EventDto>();
        }

        public IEnumerable<EventDto> Events { get; set; }

        public DateTimeOffset Timestamp { get; set; }
    }
}
