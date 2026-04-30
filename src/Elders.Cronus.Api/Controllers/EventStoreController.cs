using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using System;
using static Elders.Cronus.Api.EventStoreExplorer;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Elders.Cronus.EventStore.Index;
using Microsoft.Extensions.Logging;
using Elders.Cronus.EventStore;
using Elders.Cronus.MessageProcessing;

namespace Elders.Cronus.Api.Controllers
{
    [Route("EventStore")]
    public class EventStoreController : ApiControllerBase
    {
        private readonly EventStoreExplorer eventExplorer;
        private readonly IPublisher<IEvent> publisher;
        private readonly IPublisher<IPublicEvent> publicPublisher;
        private readonly ICronusContextAccessor cronusContextAccessor;
        private readonly ILogger<EventStoreController> logger;

        public EventStoreController(EventStoreExplorer eventStoreExplorer, IPublisher<IEvent> publisher, IPublisher<IPublicEvent> publicPublisher, ICronusContextAccessor cronusContextAccessor, ILogger<EventStoreController> logger)
        {
            if (eventStoreExplorer is null) throw new ArgumentNullException(nameof(eventStoreExplorer));

            this.eventExplorer = eventStoreExplorer;
            this.publisher = publisher;
            this.publicPublisher = publicPublisher;
            this.cronusContextAccessor = cronusContextAccessor;
            this.logger = logger;
        }

        [HttpGet, Route("Explore")]
        public async Task<IActionResult> Explore([FromQuery] RequestModel model)
        {
            AggregateDto result = new AggregateDto();
            try
            {
                result = await eventExplorer.ExploreAsync(AggregateRootId.Parse(model.Id));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to explore aggregate for {modelId}", model.Id);
            }

            return new OkObjectResult(new ResponseResult<AggregateDto>(result));
        }

        [HttpPost, Route("ExploreWithPaging")]
        public async Task<IActionResult> ExploreWithPaging([FromBody] ExploreEventStoreWithPagingRequestModel model)
        {
            ExploreWithPagingResponse result = ExploreWithPagingResponse.Empty();
            PagingOptions options = new PagingOptions(model.Take, model.PaginationToken, Order.Descending);
            try
            {
                result = await eventExplorer.ExploreEventsWithPagingAsync(AggregateRootId.Parse(model.Id), options);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to explore aggregate with paging. Id: {modelId}", model.Id);
            }

            return new OkObjectResult(new ResponseResult<ExploreWithPagingResponse>(result));
        }

        /// <summary>
        /// Republishes a previously stored event by fetching it from the event store and dispatching it through the appropriate publisher.
        /// </summary>
        /// <param name="model">The republish request describing the aggregate root, revision, position and whether the event is public.</param>
        /// <param name="cancellationToken">Propagates notification that the request should be canceled (bound to <see cref="HttpContext.RequestAborted"/>).</param>
        [HttpPost, Route("Republish")]
        public async Task<IActionResult> Republish([FromBody] RepublishRequest model, CancellationToken cancellationToken)
        {
            var arId = AggregateRootId.Parse(model.Id);

            if (model.IsPublicEvent)
            {
                IPublicEvent @event = await eventExplorer.FindPublicEventAsync(arId, model.CommitRevision, model.EventPosition);

                if (@event is null) return BadRequest("Event not found");

                Dictionary<string, string> headers = new Dictionary<string, string>()
                {
                    { MessageHeader.AggregateRootId,  arId.Value}
                };

                await publicPublisher.PublishAsync(@event, headers, cancellationToken);

            }
            else
            {
                RepublishEventData eventData = await eventExplorer.FindEventAsync(arId, model.CommitRevision, model.EventPosition);

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

                await publisher.PublishAsync(eventData.EventToRepublish, headers, cancellationToken);
            }

            return new OkObjectResult(new ResponseResult());
        }

        /// <summary>
        /// Republishes a previously stored event using the raw-bytes overload, preserving the original payload format and metadata.
        /// </summary>
        /// <param name="model">The republish request describing the aggregate, revision, position, timestamp and event contract.</param>
        /// <param name="cancellationToken">Propagates notification that the request should be canceled (bound to <see cref="HttpContext.RequestAborted"/>).</param>
        [HttpPost]
        [Route("RepublishNew")]
        public async Task<IActionResult> RepublishNew([FromBody] RepublishRequestNew model, CancellationToken cancellationToken)
        {
            AggregateRootId id = AggregateRootId.Parse(model.Id);
            IndexRecord record = new IndexRecord(model.EventContract, id.RawId, model.CommitRevision, model.EventPosition, model.Timestamp);
            AggregateEventRaw rawEvent = await eventExplorer.GetAggregateEventRaw(record).ConfigureAwait(false);

            byte[] rawData = rawEvent.Data;

            if (rawEvent is null)
                return BadRequest("Event not found");

            Type eventType = model.EventContract.GetTypeByContract();
            string tenant = cronusContextAccessor.CronusContext.Tenant;

            if (model.IsPublicEvent)
            {
                Dictionary<string, string> headers = new Dictionary<string, string>()
                {
                    { MessageHeader.AggregateRootId,  id.Value},
                    { MessageHeader.AggregateRootRevision, model.CommitRevision.ToString()},
                    { MessageHeader.AggregateRootEventPosition, model.EventPosition.ToString() },
                    { MessageHeader.AggregateCommitTimestamp, rawEvent.Timestamp.ToString() },
                };

                await publicPublisher.PublishAsync(rawData, eventType, tenant, headers, cancellationToken);
            }
            else
            {
                string recipientHandlers = ConcatRecipientHandlers(model.RecipientHandlers);
                Dictionary<string, string> headers = new Dictionary<string, string>()
                {
                    { MessageHeader.AggregateRootId,  id.Value},
                    { MessageHeader.AggregateRootRevision, model.CommitRevision.ToString()},
                    { MessageHeader.AggregateRootEventPosition, model.EventPosition.ToString() },
                    { MessageHeader.AggregateCommitTimestamp, rawEvent.Timestamp.ToString() },
                    { MessageHeader.RecipientHandlers, string.Join(',', recipientHandlers) }
                };

                await publisher.PublishAsync(rawData, eventType, tenant, headers, cancellationToken);
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

        public class RepublishRequestNew
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

            [Required]
            public long Timestamp { get; set; }

            [Required]
            public string EventContract { get; set; }
        }

        public class ExploreEventStoreWithPagingRequestModel
        {
            [Required]
            public string Id { get; set; }

            public byte[] PaginationToken { get; set; }

            public int Take { get; set; }
        }

        public class RequestModel
        {
            [Required]
            public string Id { get; set; }
        }
    }
}
