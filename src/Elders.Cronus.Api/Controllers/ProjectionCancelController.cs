using Elders.Cronus.MessageProcessing;
using Elders.Cronus.Projections;
using Elders.Cronus.Projections.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Elders.Cronus.Api.Controllers
{
    [Route("Projection")]
    public class ProjectionCancelController : ControllerBase
    {
        private readonly IPublisher<ICommand> _publisher;
        private readonly CronusContext context;

        public ProjectionCancelController(IPublisher<ICommand> publisher, CronusContext context)
        {
            if (publisher is null) throw new ArgumentNullException(nameof(publisher));
            if (context is null) throw new ArgumentNullException(nameof(context));

            _publisher = publisher;
            this.context = context;
        }

        [HttpPost, Route("Cancel")]
        public IActionResult Cancel([FromBody]RequestModel model)
        {
            var version = new Projections.ProjectionVersion(model.ProjectionContractId, ProjectionStatus.Create(model.Version.Status), model.Version.Revision, model.Version.Hash);
            var command = new CancelProjectionVersionRequest(new ProjectionVersionManagerId(model.ProjectionContractId, context.Tenant), version, model.Reason ?? "Canceled by user");

            if (_publisher.Publish(command)) return new OkObjectResult(new ResponseResult());

            return new BadRequestObjectResult(new ResponseResult<string>($"Unable to publish command '{nameof(CancelProjectionVersionRequest)}'"));
        }

        public class RequestModel
        {
            public string ProjectionContractId { get; set; }

            public ProjectionVersion Version { get; set; }

            public string Reason { get; set; }
        }
    }
}
