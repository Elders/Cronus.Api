namespace Elders.Cronus.Api.Playground.Domain.Samples
{
    public class Sample : AggregateRoot<SampleState>
    {
        Sample() { }

        public Sample(SampleId experienceId, Volume volume)
        {
            Apply(new SampleCreated(experienceId, volume));
        }

        public void Reserve()
        {
            if (state.IsNotReserved)
            {
                var @event = new SampleReserved(state.Id);
                Apply(@event);
            }
        }
    }
}
