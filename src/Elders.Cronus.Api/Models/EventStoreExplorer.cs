﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elders.Cronus.EventStore;

namespace Elders.Cronus.Api
{
    public class EventStoreExplorer
    {
        private readonly IEventStore eventStore;
        private readonly BoundedContext boundedContext;

        public EventStoreExplorer(IEventStore eventStore, BoundedContext boundedContext)
        {
            if (ReferenceEquals(null, eventStore) == true) throw new ArgumentNullException(nameof(eventStore));

            this.eventStore = eventStore;
            this.boundedContext = boundedContext;
        }

        public async Task<AggregateDto> ExploreAsync(IAggregateRootId id)
        {
            EventStream stream = eventStore.Load(id);
            if (stream.Commits.Count() == 0) return new AggregateDto();

            var commitsDto = stream.Commits.Select(commit =>
                new AggregateCommitDto()
                {
                    AggregateRootRevision = commit.Revision,
                    Events = commit.Events.Select(x => new EventDto() { EventName = x.GetType().Name, EventData = x }).ToList(),
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
            public string EventName { get; set; }

            public IEvent EventData { get; set; }
        }
    }
}
