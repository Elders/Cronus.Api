using Elders.Cronus.Discoveries;
using Elders.Cronus.Projections;
using Elders.Cronus.Projections.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Elders.Cronus.MessageProcessing;

namespace Elders.Cronus.Api.Controllers
{
    [Route("Projections")]
    public class ProjectionListController : ApiControllerBase
    {
        private readonly ProjectionExplorer _projectionExplorer;
        private readonly CronusContext context;
        private readonly ProjectionHasher projectionHasher;

        public ProjectionListController(ProjectionExplorer projectionExplorer, CronusContext context, ProjectionHasher projectionHasher)
        {
            if (projectionExplorer is null) throw new ArgumentNullException(nameof(projectionExplorer));
            if (context is null) throw new ArgumentNullException(nameof(context));

            _projectionExplorer = projectionExplorer;
            this.context = context;
            this.projectionHasher = projectionHasher;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.IsDynamic == false);

            var projectionMetaData = loadedAssemblies
                .SelectMany(ass => ass.GetLoadableTypes()
                .Where(x => typeof(IProjection).IsAssignableFrom(x) && x.GetCustomAttributes(typeof(DataContractAttribute), false).Length > 0));

            ProjectionListDto result = new ProjectionListDto();
            foreach (var meta in projectionMetaData)
            {
                var id = new ProjectionVersionManagerId(meta.GetContractId(), context.Tenant);
                var dto = await _projectionExplorer.ExploreAsync(id, typeof(ProjectionVersionsHandler));
                ProjectionVersionsHandlerState state = dto?.State as ProjectionVersionsHandlerState;
                var metaProjection = new ProjectionMeta()
                {
                    ProjectionContractId = meta.GetContractId(),
                    ProjectionName = meta.Name,
                    IsReplayable = typeof(IAmEventSourcedProjection).IsAssignableFrom(meta)
                };
                if (ReferenceEquals(null, state))
                {
                    metaProjection.Versions.Add(new ProjectionVersionDto()
                    {
                        Status = ProjectionStatus.NotPresent,
                        RebuildStatus = ProjectionRebuildStatus.Idle,
                        Hash = projectionHasher.CalculateHash(meta),
                        Revision = 0
                    });
                }
                else
                {
                    foreach (var ver in state.AllVersions)
                    {
                        metaProjection.Versions.Add(new ProjectionVersionDto()
                        {
                            Hash = ver.Hash,
                            Revision = ver.Revision,
                            Status = ver.Status,
                            RebuildStatus = ver.RebuildStatus
                        });
                    }
                }
                result.Projections.Add(metaProjection);
            }

            return new OkObjectResult(new ResponseResult<ProjectionListDto>(result));
        }
    }

    public class ProjectionListDto
    {
        public ProjectionListDto()
        {
            Projections = new List<ProjectionMeta>();
        }

        public List<ProjectionMeta> Projections { get; set; }
    }

    public class ProjectionMeta
    {
        public ProjectionMeta()
        {
            Versions = new List<ProjectionVersionDto>();
        }

        public string ProjectionContractId { get; set; }

        public string ProjectionName { get; set; }

        public bool IsReplayable { get; set; }

        public List<ProjectionVersionDto> Versions { get; set; }
    }

    public class ProjectionVersionDto
    {
        public string Hash { get; set; }

        public int Revision { get; set; }

        public string Status { get; set; }

        public string RebuildStatus { get; set; }
    }
}
