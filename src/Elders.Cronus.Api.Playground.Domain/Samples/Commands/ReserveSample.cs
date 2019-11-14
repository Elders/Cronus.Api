using System;
using System.Runtime.Serialization;

namespace Elders.Cronus.Api.Playground.Domain.Samples
{
    [DataContract(Name = "8d710982-9126-44a5-acce-e0db20d93a43")]
    public class ReserveSample : ICommand
    {
        ReserveSample() { }

        public ReserveSample(SampleId id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        [DataMember(Order = 1)]
        public SampleId Id { get; private set; }
    }
}
