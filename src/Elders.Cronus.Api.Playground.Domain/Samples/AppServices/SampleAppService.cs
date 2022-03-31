using System.Threading.Tasks;

namespace Elders.Cronus.Api.Playground.Domain.Samples.AppServices
{
    public class SampleAppService : ApplicationService<Sample>,
        ICommandHandler<CreateSample>,
        ICommandHandler<ReserveSample>
    {
        public SampleAppService(IAggregateRepository repository) : base(repository) { }

        public Task HandleAsync(CreateSample command)
        {
            var sample = new Sample(command.Id, command.Volume);
            repository.Save(sample);

            return Task.CompletedTask;
        }

        public Task HandleAsync(ReserveSample command)
        {
            Update(command.Id, ar => ar.Reserve());

            return Task.CompletedTask;
        }
    }
}
