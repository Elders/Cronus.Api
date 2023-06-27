﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using static Elders.Cronus.Api.EventStoreExplorer;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Elders.Cronus.EventStore.Index;
using Microsoft.Extensions.Logging;
using Elders.Cronus.EventStore;

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

        [HttpPost, Route("ExploreWithPaging")]
        public async Task<IActionResult> ExploreWithPaging([FromBody] ExploreEventStoreWithPagingRequestModel model)
        {
            ExploreWithPagingResponse result = ExploreWithPagingResponse.Empty();
            PagingOptions options = new PagingOptions(model.Take, model.PaginationToken);
            try
            {
                result = await _eventExplorer.ExploreEventsWithPagingAsync(AggregateRootId.Parse(model.Id, Urn.Uber), options);

            }
            catch (Exception ex)
            {
                logger.ErrorException(ex, () => $"Failed to explore aggregate with paging. Id: {model.Id}");
            }

            return new OkObjectResult(new ResponseResult<ExploreWithPagingResponse>(result));
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

        [HttpPost]
        [Route("RepublishNew")]
        public async Task<IActionResult> RepublishNew([FromBody] RepublishRequestNew model)
        {
            AggregateRootId id = AggregateRootId.Parse(model.Id, Urn.Uber);
            IndexRecord record = new IndexRecord(model.EventContract, id.RawId, model.CommitRevision, model.EventPosition, model.Timestamp);
            AggregateEventRaw rawEvent = await _eventExplorer.GetAggregateEventRaw(record).ConfigureAwait(false);

            ReadOnlyMemory<byte> rawData = rawEvent.Data;

            if (rawEvent is null)
                return BadRequest("Event not found");

            Type eventType = model.EventContract.GetTypeByContract();

            if (model.IsPublicEvent)
            {
                Dictionary<string, string> headers = new Dictionary<string, string>()
                {
                    { MessageHeader.AggregateRootId,  id.ToBase64()}
                };

                publicPublisher.Publish(rawData, eventType, headers);
            }
            else
            {
                string recipientHandlers = ConcatRecipientHandlers(model.RecipientHandlers);
                Dictionary<string, string> headers = new Dictionary<string, string>()
                {
                    { MessageHeader.AggregateRootId,  id.ToBase64()},
                    { MessageHeader.AggregateRootRevision, model.CommitRevision.ToString()},
                    { MessageHeader.AggregateRootEventPosition, model.EventPosition.ToString() },
                    { MessageHeader.AggregateCommitTimestamp, rawEvent.Timestamp.ToString() },
                    { MessageHeader.RecipientHandlers, string.Join(',', recipientHandlers) }
            };
                publisher.Publish(rawData, eventType, headers);
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
