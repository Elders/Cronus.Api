using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using static Elders.Cronus.Api.EventStoreExplorer;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Elders.Cronus.EventStore.Index;
using Microsoft.Extensions.Logging;

namespace Elders.Cronus.Api.Controllers
{
    [Route("EventStore")]
    public class EventStoreController : ApiControllerBase
    {
        private readonly EventStoreExplorer _eventExplorer;
        private readonly IPublisher<IEvent> publisher;
        private readonly IPublisher<IPublicEvent> publicPublisher;
        private readonly ILogger<EventStoreController> logger;

        public EventStoreController(EventStoreExplorer eventStoreExplorer, IPublisher<IEvent> publisher, IPublisher<IPublicEvent> publicPublisher, ILogger<EventStoreController> logger)
        {
            if (eventStoreExplorer is null) throw new ArgumentNullException(nameof(eventStoreExplorer));

            _eventExplorer = eventStoreExplorer;
            this.publisher = publisher;
            this.publicPublisher = publicPublisher;
            this.logger = logger;
        }

        [HttpGet, Route("Explore")]
        public async Task<IActionResult> Explore([FromQuery] RequestModel model)
        {
            AggregateDto result = new AggregateDto();
            try
            {
                result = await _eventExplorer.ExploreAsync(AggregateRootId.Parse(model.Id, Urn.Uber));
            }
            catch (Exception ex)
            {
                logger.ErrorException(ex, () => $"Failed to explore aggregate for {model.Id}");
            }

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
            var arId = AggregateRootId.Parse(model.Id, Urn.Uber);

            if (model.IsPublicEvent)
            {
                IPublicEvent @event = await _eventExplorer.FindPublicEventAsync(arId, model.CommitRevision, model.EventPosition);

                if (@event is null) return BadRequest("Event not found");

                Dictionary<string, string> headers = new Dictionary<string, string>()
                {
                    { MessageHeader.AggregateRootId,  arId.Value}
                };

                publicPublisher.Publish(@event, headers);

            }
            else
            {
                RepublishEventData eventData = await _eventExplorer.FindEventAsync(arId, model.CommitRevision, model.EventPosition);

                if (eventData is null) return BadRequest("Event not found");

                string recipientHandlers = ConcatRecipientHandlers(model.RecipientHandlers);

                Dictionary<string, string> headers = new Dictionary<string, string>()
                {
                    { MessageHeader.AggregateRootId,  arId.Value},
                    { MessageHeader.AggregateRootRevision, model.CommitRevision.ToString()},
                    { MessageHeader.AggregateRootEventPosition, model.EventPosition.ToString() },
                    { MessageHeader.AggregateCommitTimestamp, eventData.Timestamp.ToString() },
                    { MessageHeader.RecipientHandlers, string.Join(',', recipientHandlers) }
                };

                publisher.Publish(eventData.EventToRepublish, headers);
            }

            return new OkObjectResult(new ResponseResult());
        }

        private string ConcatRecipientHandlers(string[] chosenRecipientHandlers)
        {
            string projectionIndexContract = typeof(ProjectionIndex).GetContractId();
            string handlerContracts = string.Join(',', chosenRecipientHandlers);

            return $"{projectionIndexContract},{handlerContracts}";
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
