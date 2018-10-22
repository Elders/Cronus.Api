using Elders.Cronus.Projections.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Elders.Cronus.Api.Core.Controllers
{
    public class ProjectionRebuildController : ControllerBase
    {
        private readonly IPublisher<ICommand> _publisher;

        public ProjectionRebuildController(IPublisher<ICommand> publisher)
        {
            if (publisher is null) throw new ArgumentNullException(nameof(publisher));

            _publisher = publisher;
        }

        [HttpPost]
        public IActionResult Rebuild(RequestModel model)
        {
            var command = new RebuildProjection(new ProjectionVersionManagerId(model.ProjectionContractId), model.Hash);

            if (_publisher.Publish(command))
                return new OkObjectResult(new ResponseResult());

            return new BadRequestObjectResult(new ResponseResult<string>($"Unable to publish command '{nameof(RebuildProjection)}'"));
        }

        public class RequestModel
        {
            public string ProjectionContractId { get; set; }

            public string Hash { get; set; }
        }
    }
}
