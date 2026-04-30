using Elders.Cronus.MessageProcessing;
using Elders.Cronus.Projections;
using Elders.Cronus.Projections.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Elders.Cronus.Api.Controllers
{
    [Route("Projection")]
    public class ProjectionCancelController : ApiControllerBase
    {
        private readonly IPublisher<ICommand> _publisher;
        private readonly ICronusContextAccessor contextAccessor;

        public ProjectionCancelController(IPublisher<ICommand> publisher, ICronusContextAccessor contextAccessor)
        {
            if (publisher is null) throw new ArgumentNullException(nameof(publisher));
            if (contextAccessor is null) throw new ArgumentNullException(nameof(contextAccessor));

            _publisher = publisher;
            this.contextAccessor = contextAccessor;
        }

        /// <summary>
        /// Pauses the specified projection version.
        /// </summary>
        /// <param name="model">The request describing the projection contract id, version, and optional reason.</param>
        /// <param name="cancellationToken">Propagates notification that the request should be canceled (bound to <see cref="HttpContext.RequestAborted"/>).</param>
        [HttpPost, Route("Pause")]
        public async Task<IActionResult> Pause([FromBody] ProjcetionRequestModel model, CancellationToken cancellationToken)
        {
            var version = new Projections.ProjectionVersion(model.ProjectionContractId, ProjectionStatus.Create(model.Version.Status), model.Version.Revision, model.Version.Hash);
            var command = new PauseProjectionVersion(new ProjectionVersionManagerId(model.ProjectionContractId, contextAccessor.CronusContext.Tenant), version);

            if (await _publisher.PublishAsync(command, cancellationToken: cancellationToken))
                return new OkObjectResult(new ResponseResult());

            return new BadRequestObjectResult(new ResponseResult<string>($"Unable to publish command '{nameof(NewProjectionVersion)}'"));
        }

        /// <summary>
        /// Cancels the specified projection version.
        /// </summary>
        /// <param name="model">The request describing the projection contract id, version, and optional reason.</param>
        /// <param name="cancellationToken">Propagates notification that the request should be canceled (bound to <see cref="HttpContext.RequestAborted"/>).</param>
        [HttpPost, Route("Cancel")]
        public async Task<IActionResult> Cancel([FromBody] ProjcetionRequestModel model, CancellationToken cancellationToken)
        {
            var version = new Projections.ProjectionVersion(model.ProjectionContractId, ProjectionStatus.Create(model.Version.Status), model.Version.Revision, model.Version.Hash);
            var command = new CancelProjectionVersionRequest(new ProjectionVersionManagerId(model.ProjectionContractId, contextAccessor.CronusContext.Tenant), version, model.Reason ?? "Canceled by user");

            if (await _publisher.PublishAsync(command, cancellationToken: cancellationToken))
                return new OkObjectResult(new ResponseResult());

            return new BadRequestObjectResult(new ResponseResult<string>($"Unable to publish command '{nameof(CancelProjectionVersionRequest)}'"));
        }

        /// <summary>
        /// Finalizes the specified projection version.
        /// </summary>
        /// <param name="model">The request describing the projection contract id and version.</param>
        /// <param name="cancellationToken">Propagates notification that the request should be canceled (bound to <see cref="HttpContext.RequestAborted"/>).</param>
        [HttpPost, Route("Finalize")]
        public async Task<IActionResult> Finalize([FromBody] ProjcetionRequestModel model, CancellationToken cancellationToken)
        {
            var version = new Projections.ProjectionVersion(model.ProjectionContractId, ProjectionStatus.Create(model.Version.Status), model.Version.Revision, model.Version.Hash);
            var command = new FinalizeProjectionVersionRequest(new ProjectionVersionManagerId(model.ProjectionContractId, contextAccessor.CronusContext.Tenant), version);

            if (await _publisher.PublishAsync(command, cancellationToken: cancellationToken))
                return new OkObjectResult(new ResponseResult());

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
