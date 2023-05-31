using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elders.Cronus.EventStore;
using Elders.Cronus.Projections;
using Elders.Cronus.Projections.Cassandra.EventSourcing;
using Microsoft.Extensions.Options;

namespace Elders.Cronus.Api
{
    public class EventStoreExplorer
    {
        private readonly IEventStore eventStore;
        private readonly BoundedContext boundedContext;

        public EventStoreExplorer(IEventStore eventStore, IOptionsMonitor<BoundedContext> boundedContextMonitor)
        {
            if (ReferenceEquals(null, eventStore) == true) throw new ArgumentNullException(nameof(eventStore));

            this.eventStore = eventStore;
            this.boundedContext = boundedContextMonitor.CurrentValue;
        }

        public async Task<AggregateDto> ExploreAsync(AggregateRootId id)
        {
            EventStream stream = await eventStore.LoadAsync(id).ConfigureAwait(false);
            if (stream.Commits.Any() == false) return new AggregateDto();

            var commitsDto = stream.Commits.Select(commit => BuildAggregateCommitDto(commit)).ToList();

            var arDto = new AggregateDto()
            {
                BoundedContext = boundedContext.Name,
                AggregateId = id.Value,
                Commits = commitsDto
            };
            return arDto;
        }

        public async Task<RepublishEventData> FindEventAsync(AggregateRootId id, int commitRevision, int eventPosition)
        {
            EventStream stream = await eventStore.LoadAsync(id).ConfigureAwait(false);

            AggregateCommit commit = stream.Commits.Where(commit => commit.Revision == commitRevision).SingleOrDefault();
            if (commit is null == false)
            {
                if (commit.Events.Count > eventPosition)
                {
                    return new RepublishEventData(commit.Events[eventPosition].Unwrap(), commit.Timestamp);
                }
            }

            return null;
        }

        public class RepublishEventData
        {
            public RepublishEventData(IEvent eventToRepublish, long timestamp)
            {
                EventToRepublish = eventToRepublish;
                Timestamp = timestamp;
            }

            public IEvent EventToRepublish { get; private set; }

            public long Timestamp { get; private set; }
        }

        public async Task<IPublicEvent> FindPublicEventAsync(AggregateRootId id, int commitRevision, int eventPosition)
        {
            EventStream stream = await eventStore.LoadAsync(id).ConfigureAwait(false);

            AggregateCommit commit = stream.Commits.Where(commit => commit.Revision == commitRevision).SingleOrDefault();
            if (commit is null == false)
            {
                if (commit.Events.Count > eventPosition)
                {
                    return commit.PublicEvents[eventPosition];
                }
            }

            return null;
        }

        private AggregateCommitDto BuildAggregateCommitDto(AggregateCommit commit)
        {
            IEnumerable<EventDto> events = commit.Events.ToEventDto(DateTimeOffset.FromFileTime(commit.Timestamp));
            int lastEventPosition = events.Max(e => e.EventPosition);
            IEnumerable<EventDto> publicEvents = commit.PublicEvents.ToPublicEventDto(lastEventPosition);

            return new AggregateCommitDto()
            {
                AggregateRootRevision = commit.Revision,
                Events = events.Union(publicEvents).ToList(),
                Timestamp = DateTime.FromFileTimeUtc(commit.Timestamp)
            };
        }

        public class AggregateDto
        {
            public string BoundedContext { get; set; }

            public string AggregateId { get; set; }

            public List<AggregateCommitDto> Commits { get; set; }
        }

        public class AggregateCommitDto
        {
            public int AggregateRootRevision { get; set; }

            public List<EventDto> Events { get; set; }

            public DateTime Timestamp { get; set; }
        }
    }

    public class EventDto
    {
        public string Id { get; set; }

        public string EventName { get; set; }

        public object EventData { get; set; }

        public bool IsEntityEvent { get; set; }

        public bool IsPublicEvent { get; set; }

        public string EntityId { get; set; }

        public int EventPosition { get; set; }

        public DateTimeOffset Timestamp { get; set; }
    }

    public static class EventExtensions
    {
        public static IEnumerable<EventDto> ToEventsDto(this ProjectionCommit commit)
        {
            yield return commit.Event.ToEventDto(commit.TimeStamp);
        }

        public static ProjectionCommitDto ToProjectionDto(this ProjectionCommit commit)
        {
            return new ProjectionCommitDto()
            {
                Events = new List<EventDto> { commit.Event.ToEventDto(commit.TimeStamp) },
                Timestamp = DateTime.FromFileTimeUtc(commit.EventOrigin.Timestamp)
            };
        }

        public static EventDto ToEventDto(this IEvent @event, DateTimeOffset dateTimeOffset)
        {
            return @event.ToEventDto(dateTimeOffset, 1);
        }

        public static IEnumerable<EventDto> ToEventDto(this IEnumerable<IEvent> events, DateTimeOffset timestamp)
        {
            int eventPosition = 0;
            foreach (IEvent @event in events)
            {
                yield return @event.ToEventDto(timestamp, eventPosition);
                eventPosition++;
            }
        }

        public static IEnumerable<EventDto> ToEventDto(this List<IEvent> events, DateTimeOffset timestamp)
        {
            int eventPosition = 0;
            foreach (IEvent @event in events)
            {
                yield return @event.ToEventDto(timestamp, eventPosition);
                eventPosition++;
            }
        }

        public static EventDto ToEventDto(this IEvent @event, DateTimeOffset timestamp, int position)
        {
            var entityEvent = @event as EntityEvent;
            if (entityEvent is null)
            {
                return new EventDto()
                {
                    Id = @event.GetType().GetContractId(),
                    EventName = @event.GetType().Name,
                    EventData = @event,
                    EventPosition = position,
                    IsPublicEvent = typeof(IPublicEvent).IsAssignableFrom(@event.GetType()),
                    Timestamp = timestamp
                };
            }
            else
            {
                return new EventDto()
                {
                    Id = entityEvent.Event.GetType().GetContractId(),
                    EventName = entityEvent.Event.GetType().Name,
                    EventData = entityEvent.Event,
                    IsEntityEvent = true,
                    EventPosition = 1,
                    IsPublicEvent = typeof(IPublicEvent).IsAssignableFrom(@event.GetType()),
                    Timestamp = timestamp
                };
            }
        }

        public static IEnumerable<EventDto> ToPublicEventDto(this IEnumerable<IPublicEvent> events, int lastEventPosition)
        {
            int eventPosition = lastEventPosition + 5;
            foreach (IPublicEvent @event in events)
            {
                yield return new EventDto()
                {
                    Id = @event.GetType().GetContractId(),
                    EventName = @event.GetType().Name,
                    EventData = @event,
                    EventPosition = eventPosition,
                    IsPublicEvent = true
                };

                eventPosition++;
            }
        }
    }
}
