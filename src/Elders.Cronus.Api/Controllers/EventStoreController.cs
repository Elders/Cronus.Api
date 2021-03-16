using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using static Elders.Cronus.Api.EventStoreExplorer;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Elders.Cronus.Api.Controllers
{
    [Route("EventStore")]
    public class EventStoreController : ApiControllerBase
    {
        private readonly EventStoreExplorer _eventExplorer;
        private readonly IPublisher<IEvent> publisher;
        private readonly IPublisher<IPublicEvent> publicPublisher;

        public EventStoreController(EventStoreExplorer eventStoreExplorer, IPublisher<IEvent> publisher, IPublisher<IPublicEvent> publicPublisher)
        {
            if (eventStoreExplorer is null) throw new ArgumentNullException(nameof(eventStoreExplorer));

            _eventExplorer = eventStoreExplorer;
            this.publisher = publisher;
            this.publicPublisher = publicPublisher;
        }

        [HttpGet, Route("Explore")]
        public async Task<IActionResult> Explore([FromQuery] RequestModel model)
        {
            AggregateDto result = await _eventExplorer.ExploreAsync(AggregateUrn.Parse(model.Id, Urn.Uber));
            return new OkObjectResult(new ResponseResult<AggregateDto>(result));
        }

        public class RequestModel
        {
            [Required]
            public string Id { get; set; }
        }

        [HttpPost, Route("Republish")]
        public async Task<IActionResult> Republish([FromBody] RepublishRequest model)
        {
            if (model.IsPublicEvent)
            {
                IPublicEvent @event = await _eventExplorer.FindPublicEventAsync(AggregateUrn.Parse(model.Id, Urn.Uber), model.CommitRevision, model.EventPosition);

                if (@event is null) return BadRequest("Event not found");

                publicPublisher.Publish(@event);

            }
            else
            {
                IEvent @event = await _eventExplorer.FindEventAsync(AggregateUrn.Parse(model.Id, Urn.Uber), model.CommitRevision, model.EventPosition);

                if (@event is null) return BadRequest("Event not found");

                Dictionary<string, string> headers = new Dictionary<string, string>()
                {
                    { MessageHeader.RecipientHandlers, string.Join(',', model.RecipientHandlers) }
                };

                publisher.Publish(@event, headers);
            }

            return new OkObjectResult(new ResponseResult());
        }

        public class RepublishRequest
        {
            [Required]
            public string[] RecipientHandlers { get; set; }

            [Required]
            public string Id { get; set; }

            [Required]
            public int CommitRevision { get; set; }

            [Required]
            public int EventPosition { get; set; }

            public bool IsPublicEvent { get; set; }
        }
    }
}
