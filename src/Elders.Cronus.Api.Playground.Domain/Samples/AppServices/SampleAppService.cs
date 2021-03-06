﻿namespace Elders.Cronus.Api.Playground.Domain.Samples.AppServices
{
    public class SampleAppService : ApplicationService<Sample>,
        ICommandHandler<CreateSample>,
        ICommandHandler<ReserveSample>
    {
        public SampleAppService(IAggregateRepository repository) : base(repository) { }

        public void Handle(CreateSample command)
        {
            var sample = new Sample(command.Id, command.Volume);
            repository.Save(sample);
        }

        public void Handle(ReserveSample command)
        {
            Update(command.Id, ar => ar.Reserve());
        }
    }
}
