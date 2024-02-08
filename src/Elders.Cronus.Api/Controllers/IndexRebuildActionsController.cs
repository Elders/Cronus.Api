using System;
using System.ComponentModel.DataAnnotations;
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

        [HttpPost, Route("Rebuild")]
        public IActionResult Rebuild([FromBody] IndexRequestModel model)
        {
            var command = new RebuildIndexCommand(new EventStoreIndexManagerId(model.IndexContractId, contextAccessor.CronusContext.Tenant));

            if (_publisher.Publish(command))
                return new OkObjectResult(new ResponseResult());

            return new BadRequestObjectResult(new ResponseResult<string>($"Unable to publish command '{nameof(FinalizeEventStoreIndexRequest)}'"));
        }

        [HttpPost, Route("Finalize")]
        public IActionResult Finalize([FromBody] IndexRequestModel model)
        {
            var command = new FinalizeEventStoreIndexRequest(new EventStoreIndexManagerId(model.IndexContractId, contextAccessor.CronusContext.Tenant));

            if (_publisher.Publish(command))
                return new OkObjectResult(new ResponseResult());

            return new BadRequestObjectResult(new ResponseResult<string>($"Unable to publish command '{nameof(FinalizeEventStoreIndexRequest)}'"));
        }

        public class IndexRequestModel
        {
            [Required]
            public string IndexContractId { get; set; }
        }
    }
}
