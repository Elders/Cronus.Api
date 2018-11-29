using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using static Elders.Cronus.Api.ProjectionExplorer;

namespace Elders.Cronus.Api.Controllers
{
    [Route("Projection")]
    public class ProjectionController : ControllerBase
    {
        private readonly ProjectionExplorer _projectionExplorer;

        public ProjectionController(ProjectionExplorer projectionExplorer)
        {
            if (projectionExplorer is null) throw new ArgumentNullException(nameof(projectionExplorer));

            _projectionExplorer = projectionExplorer;
        }

        [HttpGet, Route("Explore")]
        public async Task<IActionResult> Explore([FromQuery]RequestModel model)
        {
            var projectionType = model.ProjectionName.GetTypeByContract();
            ProjectionDto result = await _projectionExplorer.ExploreAsync(model.Id.ToStringTenantId(), projectionType);
            return new OkObjectResult(new ResponseResult<ProjectionDto>(result));
        }

        public class RequestModel
        {
            public string Id { get; set; }

            public string ProjectionName { get; set; }
        }
    }
}
