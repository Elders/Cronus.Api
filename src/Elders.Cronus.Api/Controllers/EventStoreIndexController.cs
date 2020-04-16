using Microsoft.AspNetCore.Mvc;
using Elders.Cronus.EventStore.Index;
using System.Collections.Generic;
using Elders.Cronus.MessageProcessing;
using Elders.Cronus.EventStore.Index.Handlers;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Elders.Cronus.Api.Controllers
{
    [Route("EventStore/Index")]
    public class EventStoreIndexController : ApiControllerBase
    {
        private readonly TypeContainer<IEventStoreIndex> endicesTypes;
        private readonly IPublisher<ICommand> publisher;
        private readonly ProjectionExplorer projection;
        private readonly CronusContext context;

        public EventStoreIndexController(CronusContext context, TypeContainer<IEventStoreIndex> endicesTypes, IPublisher<ICommand> publisher, ProjectionExplorer projection)
        {
            this.context = context;
            this.endicesTypes = endicesTypes;
            this.publisher = publisher;
            this.projection = projection;
        }

        [HttpGet, Route("Meta")]
        public async Task<IActionResult> Meta()
        {
            List<MetaResponseModel> result = new List<MetaResponseModel>();

            foreach (var index in endicesTypes.Items)
            {
                var status = await projection.ExploreAsync(new EventStoreIndexManagerId(index.GetContractId(), context.Tenant), typeof(EventStoreIndexStatus));

                MetaResponseModel indexResponse = new MetaResponseModel()
                {
                    Id = index.GetContractId(),
                    Name = index.Name,
                    Status = status.State.ToString()
                };

                result.Add(indexResponse);
            }

            return new OkObjectResult(new ResponseResult<List<MetaResponseModel>>(result));
        }

        [HttpPost, Route("Rebuild")]
        public IActionResult Rebuild([FromBody]RebuildIndexRequestModel model)
        {
            var command = new RebuildIndex(new EventStoreIndexManagerId(model.Id, context.Tenant));

            if (publisher.Publish(command))
                return new OkObjectResult(new ResponseResult());

            return new BadRequestObjectResult(new ResponseResult<string>($"Unable to publish command '{nameof(RebuildIndex)}'"));
        }

        public class MetaResponseModel
        {
            public MetaResponseModel()
            {
                Status = IndexStatus.NotPresent;
            }

            public string Id { get; set; }

            public string Name { get; set; }

            public string Status { get; set; }
        }

        public class RebuildIndexRequestModel
        {
            [Required]
            public string Id { get; set; }
        }
    }
}
