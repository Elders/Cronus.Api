using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elders.Cronus.EventStore;
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

        public async Task<AggregateDto> ExploreAsync(IAggregateRootId id)
        {
            EventStream stream = eventStore.Load(id);
            if (stream.Commits.Count() == 0) return new AggregateDto();

            var commitsDto = stream.Commits.Select(commit =>
                new AggregateCommitDto()
                {
                    AggregateRootRevision = commit.Revision,
                    Events = commit.Events.ToEventDto().Union(commit.PublicEvents.ToEventDto()).ToList(),
                    Timestamp = DateTime.FromFileTimeUtc(commit.Timestamp)
                }).ToList();

            var arDto = new AggregateDto()
            {
                BoundedContext = boundedContext.Name,
                AggregateId = id.Value,
                Commits = commitsDto
            };
            return arDto;
        }

        public async Task<IEvent> FindEventAsync(IAggregateRootId id, int commitRevision, int eventPosition)
        {
            EventStream stream = eventStore.Load(id);

            AggregateCommit commit = stream.Commits.Where(commit => commit.Revision == commitRevision).SingleOrDefault();
            if (commit is null == false)
            {
                if (commit.Events.Count > eventPosition)
                {
                    return commit.Events[eventPosition].Unwrap();
                }
            }

            return null;
        }

        public async Task<IPublicEvent> FindPublicEventAsync(IAggregateRootId id, int commitRevision, int eventPosition)
        {
            EventStream stream = eventStore.Load(id);

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

        public class EventDto
        {
            public string Id { get; set; }

            public string EventName { get; set; }

            public object EventData { get; set; }

            public bool IsEntityEvent { get; set; }

            public bool IsPublicEvent { get; set; }

            public string EntityId { get; set; }

            public int EventPosition { get; set; }
        }
    }

    public static class EventExtensions
    {
        public static IEnumerable<EventStoreExplorer.EventDto> ToEventDto(this List<IEvent> events)
        {
            int eventPosition = 0;
            foreach (IEvent @event in events)
            {
                var entityEvent = @event as EntityEvent;
                if (ReferenceEquals(null, entityEvent))
                {
                    yield return new EventStoreExplorer.EventDto()
                    {
                        Id = @event.GetType().GetContractId(),
                        EventName = @event.GetType().Name,
                        EventData = @event,
                        EventPosition = eventPosition
                    };
                }
                else
                {
                    yield return new EventStoreExplorer.EventDto()
                    {
                        Id = entityEvent.Event.GetType().GetContractId(),
                        EventName = entityEvent.Event.GetType().Name,
                        EventData = entityEvent.Event,
                        IsEntityEvent = true,
                        EventPosition = eventPosition
                    };
                }
                eventPosition++;
            }

        }

        public static IEnumerable<EventStoreExplorer.EventDto> ToEventDto(this IEnumerable<IPublicEvent> events)
        {
            foreach (IEvent @event in events)
            {
                yield return new EventStoreExplorer.EventDto()
                {
                    Id = @event.GetType().GetContractId(),
                    EventName = @event.GetType().Name,
                    EventData = @event,
                    IsPublicEvent = true
                };
            }
        }
    }
}
