using Elders.Cronus.Projections;
using Elders.Cronus.Projections.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Elders.Cronus.Api.Core.Controllers
{
    public class ProjectionCancelController : ControllerBase
    {
        private readonly IPublisher<ICommand> _publisher;

        public ProjectionCancelController(IPublisher<ICommand> publisher)
        {
            if (publisher is null) throw new ArgumentNullException(nameof(publisher));

            _publisher = publisher;
        }

        [HttpPost]
        public IActionResult Cancel(RequestModel model)
        {
            var version = new Projections.ProjectionVersion(model.ProjectionContractId, ProjectionStatus.Create(model.Version.Status), model.Version.Revision, model.Version.Hash);
            var command = new CancelProjectionVersionRequest(new ProjectionVersionManagerId(model.ProjectionContractId), version, model.Reason ?? "Canceled by user");

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
