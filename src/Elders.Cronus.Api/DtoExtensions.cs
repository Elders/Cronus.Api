using Elders.Cronus.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Elders.Cronus.Api.ProjectionExplorer;

namespace Elders.Cronus.Api
{
    public static class DtoExtensions
    {
        public static IEnumerable<ProjectionEventDto> ExtractEventData(this IEnumerable<ProjectionCommit> projectionCommits)
        {
            foreach (ProjectionCommit commit in projectionCommits)
            {
                yield return new ProjectionEventDto { Data = commit.Event, Timestamp = commit.TimeStamp };
            }
        }
    }
}
