using Elders.Cronus;
using Elders.Cronus.Projections;
using Elders.Locus.Samples.Events;
using System.Runtime.Serialization;

namespace Elders.Locus.Samples.Projections
{
    [DataContract(Name = "2cb8a10c-1ca0-41cd-a451-53dc520a4085")]
    public class IsSampleReservedProjection : ProjectionDefinition<SampleApiProjectionState, SampleId>,
        IEventHandler<SampleReserved>
    {
        public IsSampleReservedProjection()
        {
            Subscribe<SampleReserved>(x => x.Id);
        }

        public void Handle(SampleReserved @event)
        {
            State.IsReserved = true;
        }

        [DataContract(Name = "0d4d9eda-6489-4cee-9303-0e931bb146d4")]
        public class IsSampleReservedState
        {
            [DataMember(Order = 1)]
            public bool IsReserved { get; set; }
        }
    }
}
