using System;

namespace Elders.Cronus.Api.Playground.Domain.Samples.Gateways
{
    public class SampleGateway : IGateway,
        IEventHandler<SampleReserved>
    {
        public void Handle(SampleReserved @event)
        {
            Console.WriteLine($"Sample with ID: '{@event.Id}' was reserved!");
        }
    }
}
