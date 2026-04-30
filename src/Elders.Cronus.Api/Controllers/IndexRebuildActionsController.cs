using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Elders.Cronus.EventStore.Index;
using Elders.Cronus.MessageProcessing;
using Microsoft.AspNetCore.Mvc;

namespace Elders.Cronus.Api.Controllers
{
    [Route("Index")]
    public class IndexRebuildActionsController : ApiControllerBase
    {
        private readonly IPublisher<ICommand> _publisher;
        private readonly ICronusContextAccessor contextAccessor;

        public IndexRebuildActionsController(IPublisher<ICommand> publisher, ICronusContextAccessor contextAccessor)
        {
            if (publisher is null) throw new ArgumentNullException(nameof(publisher));

            _publisher = publisher;
            this.contextAccessor = contextAccessor;
        }

        /// <summary>
        /// Issues a <see cref="RebuildIndexCommand"/> for the specified event-store index.
        /// </summary>
        /// <param name="model">The request describing the index contract id and optional max degree of parallelism.</param>
        /// <param name="cancellationToken">Propagates notification that the request should be canceled (bound to <see cref="HttpContext.RequestAborted"/>).</param>
        [HttpPost, Route("Rebuild")]
        public async Task<IActionResult> Rebuild([FromBody] IndexRequestModel model, CancellationToken cancellationToken)
        {
            var command = new RebuildIndexCommand(new EventStoreIndexManagerId(model.IndexContractId, contextAccessor.CronusContext.Tenant), model.MaxDegreeOfParallelism);

            if (await _publisher.PublishAsync(command, cancellationToken: cancellationToken))
                return new OkObjectResult(new ResponseResult());

            return new BadRequestObjectResult(new ResponseResult<string>($"Unable to publish command '{nameof(FinalizeEventStoreIndexRequest)}'"));
        }

        /// <summary>
        /// Issues a <see cref="FinalizeEventStoreIndexRequest"/> for the specified event-store index.
        /// </summary>
        /// <param name="model">The request describing the index contract id.</param>
        /// <param name="cancellationToken">Propagates notification that the request should be canceled (bound to <see cref="HttpContext.RequestAborted"/>).</param>
        [HttpPost, Route("Finalize")]
        public async Task<IActionResult> Finalize([FromBody] IndexRequestModel model, CancellationToken cancellationToken)
        {
            var command = new FinalizeEventStoreIndexRequest(new EventStoreIndexManagerId(model.IndexContractId, contextAccessor.CronusContext.Tenant));

            if (await _publisher.PublishAsync(command, cancellationToken: cancellationToken))
                return new OkObjectResult(new ResponseResult());

            return new BadRequestObjectResult(new ResponseResult<string>($"Unable to publish command '{nameof(FinalizeEventStoreIndexRequest)}'"));
        }

        public class IndexRequestModel
        {
            [Required]
            public string IndexContractId { get; set; }

            public int? MaxDegreeOfParallelism { get; set; }
        }
    }
}
