using System;

namespace Elders.Cronus.Api.Playground.Domain.Samples.Ports
{
    public class SamplePort : IPort,
        IEventHandler<SampleReserved>
    {
        public SamplePort(IPublisher<ICommand> commandPublisher)
        {
            CommandPublisher = commandPublisher;
        }

        public IPublisher<ICommand> CommandPublisher { get; set; }

        public void Handle(SampleReserved @event)
        {
            Console.WriteLine($"Sample with ID: '{@event.Id}' was reserved!");
        }
    }
}
