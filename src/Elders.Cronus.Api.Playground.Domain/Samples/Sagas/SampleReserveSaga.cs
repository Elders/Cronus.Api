namespace Elders.Cronus.Api.Playground.Domain.Samples.Sagas
{
    public class SampleReserveSaga : Saga,
        IEventHandler<SampleCreated>
    {

        public SampleReserveSaga(IPublisher<ICommand> commandPublisher, IPublisher<IScheduledMessage> timeoutRequestPublisher)
            : base(commandPublisher, timeoutRequestPublisher)
        {
        }

        public void Handle(SampleCreated @event)
        {
            var cmd = new ReserveSample(@event.Id);

            commandPublisher.Publish(cmd);
        }
    }
}
