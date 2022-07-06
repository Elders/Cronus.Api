using Elders.Cronus.MessageProcessing;
using Elders.Cronus.Projections;
using Elders.Cronus.Projections.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Elders.Cronus.Api.Controllers
{
    [Route("Projection")]
    public class ProjectionCancelController : ApiControllerBase
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
        public IActionResult Cancel([FromBody] ProjcetionRequestModel model)
        {
            var version = new Projections.ProjectionVersion(model.ProjectionContractId, ProjectionStatus.Create(model.Version.Status), model.Version.Revision, model.Version.Hash);
            var command = new CancelProjectionVersionRequest(new ProjectionVersionManagerId(model.ProjectionContractId, context.Tenant), version, model.Reason ?? "Canceled by user");

            if (_publisher.Publish(command)) return new OkObjectResult(new ResponseResult());

            return new BadRequestObjectResult(new ResponseResult<string>($"Unable to publish command '{nameof(CancelProjectionVersionRequest)}'"));
        }

        [HttpPost, Route("Finalize")]
        public IActionResult Finalize([FromBody] ProjcetionRequestModel model)
        {
            var version = new Projections.ProjectionVersion(model.ProjectionContractId, ProjectionStatus.Create(model.Version.Status), model.Version.Revision, model.Version.Hash);
            var command = new FinalizeProjectionVersionRequest(new ProjectionVersionManagerId(model.ProjectionContractId, context.Tenant), version);

            if (_publisher.Publish(command)) return new OkObjectResult(new ResponseResult());

            return new BadRequestObjectResult(new ResponseResult<string>($"Unable to publish command '{nameof(FinalizeProjectionVersionRequest)}'"));
        }

        public class ProjcetionRequestModel
        {
            [Required]
            public string ProjectionContractId { get; set; }

            [Required]
            public ProjectionVersionDto Version { get; set; }

            [Required]
            public string Reason { get; set; }
        }
    }
}
