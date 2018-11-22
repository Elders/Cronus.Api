using Elders.Cronus.MessageProcessing;
using Elders.Cronus.Projections.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Elders.Cronus.Api.Controllers
{
    [Route("Projection")]
    public class ProjectionRebuildController : ControllerBase
    {
        private readonly IPublisher<ICommand> _publisher;
        private readonly CronusContext context;

        public ProjectionRebuildController(IPublisher<ICommand> publisher, CronusContext context)
        {
            if (publisher is null) throw new ArgumentNullException(nameof(publisher));

            _publisher = publisher;
            this.context = context;
        }

        [HttpPost, Route("Rebuild")]
        public IActionResult Rebuild([FromBody]RequestModel model)
        {
            var command = new RebuildProjection(new ProjectionVersionManagerId(model.ProjectionContractId, context.Tenant), model.Hash);

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
