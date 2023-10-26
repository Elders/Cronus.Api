using System;
using System.Collections.Generic;
using Elders.Cronus.Projections;

namespace Elders.Cronus.Api
{
    public static class EventExtensions
    {
        public static IEnumerable<EventDto> ToEventsDto(this ProjectionCommit commit)
        {
            yield return commit.Event.ToEventDto(commit.TimeStamp);
        }

        public static ProjectionCommitDto ToProjectionDto(this ProjectionCommitPreview commit)
        {
            return new ProjectionCommitDto()
            {
                Events = new List<EventDto> { commit.Event.ToEventDto(commit.Event.Timestamp) },
                Timestamp = commit.Event.Timestamp
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

        public static RawEventDto ToRawEventDto(this IMessage @event, DateTimeOffset timestamp, int position, int revision)
        {
            var entityEvent = @event as EntityEvent;
            if (entityEvent is null)
            {
                return new RawEventDto()
                {
                    Id = @event.GetType().GetContractId(),
                    EventName = @event.GetType().Name,
                    EventData = @event,
                    IsEntityEvent = false,
                    EventPosition = position,
                    EventRevision = revision,
                    IsPublicEvent = typeof(IPublicEvent).IsAssignableFrom(@event.GetType()),
                    Timestamp = timestamp
                };
            }
            else
            {
                return new RawEventDto()
                {
                    Id = entityEvent.Event.GetType().GetContractId(),
                    EventName = entityEvent.Event.GetType().Name,
                    EventData = entityEvent.Event,
                    IsEntityEvent = true,
                    EventPosition = position,
                    EventRevision = revision,
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
