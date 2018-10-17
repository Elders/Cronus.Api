using Microsoft.AspNetCore.Mvc;
using Elders.Cronus.Api.Core;
using static Elders.Cronus.Api.Core.ProjectionExplorer;
using System.Threading.Tasks;

namespace Elders.Cronus.Api.Core.Controllers
{
    [Route("Projection")]
    public class ProjectionController : ControllerBase
    {
        public ProjectionExplorer ProjectionExplorer { get; set; }

        [HttpGet, Route("Explore")]
        public async Task<IActionResult> Explore(RequestModel model)
        {
            var projectionType = model.ProjectionContractId.GetTypeByContract();
            ProjectionDto result = await ProjectionExplorer.ExploreAsync(model.Id, projectionType);
            return new OkObjectResult(new ResponseResult<ProjectionDto>(result));
        }

        public class RequestModel
        {
            public StringTenantId Id { get; set; }

            public string ProjectionContractId { get; set; }
        }
    }
}
