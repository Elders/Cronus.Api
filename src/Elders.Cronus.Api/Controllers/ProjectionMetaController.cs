using System.Web.Http;
using System;
using System.Linq;
using Elders.Cronus.Projections;
using System.Runtime.Serialization;
using System.Web.Http.ModelBinding;
using Elders.Web.Api;
using Elders.Cronus.Projections.Versioning;

namespace Elders.Cronus.Api.Controllers
{
    [RoutePrefix("ProjectionMeta")]
    public class ProjectionMetaController : ApiController
    {
        public ProjectionExplorer ProjectionExplorer { get; set; }

        [HttpGet, Route]
        public IHttpActionResult Meta(RequestModel model)
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(ass => ass.IsDynamic == false);
            var projectionMetaData = loadedAssemblies.SelectMany(ass => ass.GetExportedTypes().Where(x => typeof(IProjectionDefinition).IsAssignableFrom(x) && x.GetCustomAttributes(typeof(DataContractAttribute), false).Length > 0));
            var meta = projectionMetaData.FirstOrDefault(x => x.GetContractId() == model.ProjectionContractId);
            if (ReferenceEquals(null, meta))
                return Ok(new ResponseResult($"Projection with contract '{model.ProjectionContractId}' not found"));

            var id = new ProjectionVersionManagerId(model.ProjectionContractId);
            var dto = ProjectionExplorer.Explore(id, typeof(ProjectionVersionsHandler));
            var state = dto?.State as ProjectionVersionsHandlerState;

            var metaProjection = new ProjectionMeta()
            {
                ProjectionContractId = meta.GetContractId(),
                ProjectionName = meta.Name,
            };

            metaProjection.Versions = state.AllVersions
                .Select(ver => new ProjectionVersion()
                {
                    Hash = ver.Hash,
                    Revision = ver.Revision.ToString(),
                    Status = ver.Status
                })
                .ToList();

            return Ok(new ResponseResult<ProjectionMeta>(metaProjection));
        }

        [ModelBinder(typeof(UrlBinder))]
        public class RequestModel
        {
            public string ProjectionContractId { get; set; }
        }
    }
}
