using System.Runtime.Serialization;

namespace Elders.Cronus.Api.Playground.Domain.Samples
{
    [DataContract(Name = "c44ad7ac-a64c-4751-973d-735db52fa9d5")]
    public class SampleReserved : IEvent
    {
        private SampleReserved() { }
        public SampleReserved(SampleId id)
        {
            Id = id;
        }

        [DataMember(Order = 1)]
        public SampleId Id { get; set; }
    }
}
