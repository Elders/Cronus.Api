using Elders.Cronus.Discoveries;
using Elders.Cronus.Projections;
using Elders.Cronus.Projections.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Elders.Cronus.MessageProcessing;

namespace Elders.Cronus.Api.Controllers
{
    [Route("ProjectionMeta")]
    public class ProjectionMetaController : ControllerBase
    {
        private readonly ProjectionExplorer _projectionExplorer;
        private readonly CronusContext context;

        public ProjectionMetaController(ProjectionExplorer projectionExplorer, CronusContext context)
        {
            if (projectionExplorer is null) throw new ArgumentNullException(nameof(projectionExplorer));

            _projectionExplorer = projectionExplorer;
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Meta(RequestModel model)
        {
            IEnumerable<Assembly> loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.IsDynamic == false);
            IEnumerable<Type> projectionMetaData = loadedAssemblies
                .SelectMany(assembly => assembly.GetLoadableTypes()
                .Where(x => typeof(IProjectionDefinition).IsAssignableFrom(x) && x.GetCustomAttributes(typeof(DataContractAttribute), false).Length > 0));

            Type metadata = projectionMetaData.FirstOrDefault(x => x.GetContractId() == model.ProjectionContractId);

            if (metadata is null) return new BadRequestObjectResult(new ResponseResult<string>($"Projection with contract '{model.ProjectionContractId}' not found"));

            var id = new ProjectionVersionManagerId(model.ProjectionContractId, context.Tenant);
            ProjectionExplorer.ProjectionDto dto = await _projectionExplorer.ExploreAsync(id, typeof(ProjectionVersionsHandler));
            var state = dto?.State as ProjectionVersionsHandlerState;

            var metaProjection = new ProjectionMeta()
            {
                ProjectionContractId = metadata.GetContractId(),
                ProjectionName = metadata.Name,
            };

            metaProjection.Versions = state.AllVersions
                .Select(ver => new ProjectionVersion()
                {
                    Hash = ver.Hash,
                    Revision = ver.Revision,
                    Status = ver.Status
                })
                .ToList();

            return Ok(new ResponseResult<ProjectionMeta>(metaProjection));
        }

        public class RequestModel
        {
            public string ProjectionContractId { get; set; }
        }
    }
}
