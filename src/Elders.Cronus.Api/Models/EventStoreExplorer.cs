﻿using System;
using System.Collections.Generic;
using System.Linq;
using Elders.Cronus.EventStore;
using Elders.Cronus.Projections;

namespace Elders.Cronus.Api
{
    public class ProjectionExplorer
    {
        private readonly IProjectionRepository projections;

        public ProjectionExplorer(IProjectionRepository projections)
        {
            if (ReferenceEquals(null, projections) == true) throw new ArgumentNullException(nameof(projections));

            this.projections = projections;
        }

        public ProjectionDto Explore(IBlobId id, Type projectionType)
        {
            var result = new ProjectionDto();
            var projectionResult = projections.Get(id, projectionType);
            if (projectionResult.Success)
            {
                result.Name = projectionType.Name;
                result.State = projectionResult.Projection.State;
            }

            return result;
        }

        public class ProjectionDto
        {
            public string Name { get; set; }
            public object State { get; set; }
        }
    }

    public class EventStoreExplorer
    {
        private readonly IEventStore eventStore;
        private readonly string boundedContext;

        public EventStoreExplorer(IEventStore eventStore, string boundedContext)
        {
            if (ReferenceEquals(null, eventStore) == true) throw new ArgumentNullException(nameof(eventStore));

            this.eventStore = eventStore;
            this.boundedContext = boundedContext;
        }

        public AggregateDto Explore(IAggregateRootId id)
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
                BoundedContext = boundedContext,
                AggregateId = id.Urn.Value,
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
