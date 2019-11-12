using System.Runtime.Serialization;

namespace Elders.Cronus.Api.Playground.Domain.Samples
{
    [DataContract(Name = "ca35eedc-9e38-48cd-85e9-a2aea51e70ad")]
    public class SampleCreated : IEvent
    {
        SampleCreated() { }

        public SampleCreated(SampleId id, Volume volume)
        {
            Id = id;
            Volume = volume;
        }

        [DataMember(Order = 1)]
        public SampleId Id { get; private set; }

        [DataMember(Order = 2)]
        public Volume Volume { get; private set; }
    }
}
