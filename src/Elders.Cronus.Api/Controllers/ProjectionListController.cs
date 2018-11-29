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
    public class ProjectionListController : ControllerBase
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
                .Where(x => typeof(IProjectionDefinition).IsAssignableFrom(x) && x.GetCustomAttributes(typeof(DataContractAttribute), false).Length > 0));

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
                };
                if (ReferenceEquals(null, state))
                {
                    metaProjection.Versions.Add(new ProjectionVersion()
                    {
                        Status = ProjectionStatus.NotPresent,
                        Hash = projectionHasher.CalculateHash(meta),
                        Revision = 0
                    });
                }
                else
                {
                    metaProjection.Versions = state.AllVersions
                        .Select(ver => new ProjectionVersion()
                        {
                            Hash = ver.Hash,
                            Revision = ver.Revision,
                            Status = ver.Status
                        })
                        .ToList();
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
            Versions = new List<ProjectionVersion>();
        }

        public string ProjectionContractId { get; set; }

        public string ProjectionName { get; set; }

        public List<ProjectionVersion> Versions { get; set; }
    }

    public class ProjectionVersion
    {
        public string Hash { get; set; }

        public int Revision { get; set; }

        public string Status { get; set; }
    }
}
