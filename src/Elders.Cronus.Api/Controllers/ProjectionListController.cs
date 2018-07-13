using System.Web.Http;
using Elders.Web.Api;
using System;
using System.Linq;
using Elders.Cronus.Projections;
using Elders.Cronus.Projections.Versioning;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Elders.Cronus.Api.Controllers
{
    [RoutePrefix("ProjectionList")]
    public class ProjectionListController : ApiController
    {
        public ProjectionExplorer ProjectionExplorer { get; set; }

        [HttpGet]
        public ResponseResult<ProjectionListDto> List()
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(ass => ass.IsDynamic == false);

            var projectionMetaData = loadedAssemblies
                .SelectMany(ass => ass.GetExportedTypes()
                    .Where(x => typeof(ISystemProjection).IsAssignableFrom(x) == false)
                    .Where(x => typeof(IProjectionDefinition).IsAssignableFrom(x) && x.GetCustomAttributes(typeof(DataContractAttribute), false).Length > 0));

            ProjectionListDto result = new ProjectionListDto();
            foreach (var meta in projectionMetaData)
            {
                var id = new ProjectionVersionManagerId(meta.GetContractId());
                var dto = ProjectionExplorer.Explore(id, typeof(ProjectionVersionsHandler));
                ProjectionVersionsHandlerState state = dto?.State as ProjectionVersionsHandlerState;
                if (ReferenceEquals(null, state)) continue;

                var metaProjection = new ProjectionMeta()
                {
                    ProjectionContractId = meta.GetContractId(),
                    ProjectionName = meta.Name,
                };

                metaProjection.Versions = state.AllVersions
                    .Select(ver => new ProjectionVersion()
                    {
                        Hash = ver.Hash,
                        Revision = ver.Revision,
                        Status = ver.Status
                    })
                    .ToList();

                result.Projections.Add(metaProjection);

            }

            return new ResponseResult<ProjectionListDto>(result);
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
