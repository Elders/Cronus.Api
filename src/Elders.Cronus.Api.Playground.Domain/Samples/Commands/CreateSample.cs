using Elders.Cronus;
using System;
using System.Runtime.Serialization;

namespace Elders.Locus.Samples.Commands
{
    [DataContract(Name = "f2e74744-4bf5-4a21-bdd8-ab3ad5f4d951")]
    public class CreateSample : ICommand
    {
        CreateSample() { }

        public CreateSample(SampleId id, Volume volume)
        {
            Id = id;
            Volume = volume;
        }

        [DataMember(Order = 1)]
        public SampleId Id { get; private set; }

        [DataMember(Order = 2)]
        public Volume Volume { get; set; }
    }
}
