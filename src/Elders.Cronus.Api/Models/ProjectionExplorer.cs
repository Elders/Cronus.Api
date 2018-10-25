using System;
using System.Threading.Tasks;
using Elders.Cronus.Projections;

namespace Elders.Cronus.Api
{
    public class ProjectionExplorer
    {
        private readonly IProjectionReader projections;

        public ProjectionExplorer(IProjectionReader projections)
        {
            if (ReferenceEquals(null, projections) == true) throw new ArgumentNullException(nameof(projections));

            this.projections = projections;
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

        public class ProjectionDto
        {
            public string Name { get; set; }
            public object State { get; set; }
        }
    }
}
