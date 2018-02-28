using System.Web.Http;
using Elders.Cronus;
using Elders.Web.Api;
using System.Web.Http.ModelBinding;
using static Elders.Cronus.Api.ProjectionExplorer;
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

            var projectionMetaData = loadedAssemblies.SelectMany(ass => ass.GetExportedTypes().Where(x => typeof(IProjectionDefinition).IsAssignableFrom(x) && x.GetCustomAttributes(typeof(DataContractAttribute), false).Length > 0));
            ProjectionListDto result = new ProjectionListDto();
            foreach (var meta in projectionMetaData)
            {
                var id = new ProjectionVersionManagerId(meta.GetContractId());
                var dto = ProjectionExplorer.Explore(id, typeof(PersistentProjectionVersionHandler));
                if (ReferenceEquals(null, dto?.State)) continue;

                var metaProjection = new ProjectionMeta()
                {
                    ProjectionContractId = meta.GetContractId(),
                    ProjectionName = meta.Name,
                };

                dynamic liveVersion = ((dynamic)dto.State).Live;
                if (ReferenceEquals(null, liveVersion) == false)
                {
                    metaProjection.Versions.Add(new ProjectionVersion()
                    {
                        Hash = liveVersion.Hash,
                        Revision = liveVersion.Revision.ToString(),
                        Status = liveVersion.Status,
                    });
                }

                dynamic buildingVersion = ((dynamic)dto.State).Building;
                if (ReferenceEquals(null, buildingVersion) == false)
                {
                    metaProjection.Versions.Add(new ProjectionVersion()
                    {
                        Hash = buildingVersion.Hash,
                        Revision = buildingVersion.Revision.ToString(),
                        Status = buildingVersion.Status,
                    });
                }

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

        public string Revision { get; set; }

        public string Status { get; set; }
    }
}
