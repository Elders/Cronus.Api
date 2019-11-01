﻿using Elders.Cronus;
using Elders.Cronus.Projections;
using Elders.Locus.Samples.Events;
using System.Runtime.Serialization;

namespace Elders.Locus.Samples.Projections
{
    /// <summary>
    /// A Projection which serves the SampleApi specific data requirements.
    /// </summary>
    /// <remarks>
    /// By applyingthe <see cref="DataContractAttribute"/> to the <see cref="SampleApiProjection"/> class
    /// you can rename the class at any point in time even after a production release.
    /// </remarks>
    [DataContract(Name = "f5bccb43-99be-4639-a9b7-1cb46e80dc7a")]
    public class SampleApiProjection : ProjectionDefinition<SampleApiProjectionState, SampleId>,
        IEventHandler<SampleCreated>,
        IEventHandler<SampleReserved>
    {
        //  * AVOID BIG collections with Cronus projections. Use ElasticSearch for search purposes.
        // List<string> collection = new List<string>();

        // * NEVER query other projections. EVER!
        // public SampleApiProjection(IProjectionReader projectionReader) { }

        public SampleApiProjection()
        {
            Subscribe<SampleCreated>(x => x.Id);
            Subscribe<SampleReserved>(x => x.Id);
        }

        public void Handle(SampleCreated @event)
        {
            State.Id = @event.Id;
        }

        public void Handle(SampleReserved @event)
        {
            State.Id = @event.Id;
            State.IsReserved = true;
        }
    }


    [DataContract(Name = "f197a9f7-e2cd-4d53-9ac0-97e582cd37e4")]
    public class SampleApiProjectionState
    {
        [DataMember(Order = 1)]
        public SampleId Id { get; set; }

        [DataMember(Order = 2)]
        public bool IsReserved { get; set; }
    }
}
