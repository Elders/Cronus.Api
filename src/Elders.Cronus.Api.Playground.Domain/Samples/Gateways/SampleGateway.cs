using System;
using System.Threading.Tasks;

namespace Elders.Cronus.Api.Playground.Domain.Samples.Gateways
{
    public class SampleGateway : IGateway,
        IEventHandler<SampleReserved>
    {
        public Task HandleAsync(SampleReserved @event)
        {
            Console.WriteLine($"Sample with ID: '{@event.Id}' was reserved!");
            return Task.CompletedTask;
        }
    }
}
