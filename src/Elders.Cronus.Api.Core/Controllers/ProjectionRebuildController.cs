using Elders.Cronus.Projections.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Elders.Cronus.Api.Core.Controllers
{
    public class ProjectionRebuildController : ControllerBase
    {
        public IPublisher<ICommand> Publisher { get; set; }

        [HttpPost]
        public IActionResult Rebuild(RequestModel model)
        {
            var command = new RebuildProjection(new ProjectionVersionManagerId(model.ProjectionContractId), model.Hash);

            if (Publisher.Publish(command))
                return new OkObjectResult(new ResponseResult<string>("asd"));

            return new BadRequestObjectResult(new ResponseResult<string>($"Unable to publish command '{nameof(RebuildProjection)}'"));
        }

        public class RequestModel
        {
            public string ProjectionContractId { get; set; }

            public string Hash { get; set; }
        }
    }
}
