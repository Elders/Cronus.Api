using Elders.Cronus.EventStore.Players;
using Elders.Cronus.MessageProcessing;
using Elders.Cronus.Projections.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Elders.Cronus.Api.Controllers
{
    [Route("Projection")]
    public class ProjectionRebuildController : ApiControllerBase
    {
        private readonly IPublisher<ICommand> _publisher;
        private readonly ICronusContextAccessor contextAccessor;

        public ProjectionRebuildController(IPublisher<ICommand> publisher, ICronusContextAccessor contextAccessor)
        {
            if (publisher is null) throw new ArgumentNullException(nameof(publisher));

            _publisher = publisher;
            this.contextAccessor = contextAccessor;
        }

        [HttpPost, Route("Rebuild")]
        public IActionResult Rebuild([FromBody] RequestModel model)
        {
            model.PlayerOptions ??= new PlayerOptions();
            var replayEventsOptions = new ReplayEventsOptions()
            {
                After = model.PlayerOptions.After,
                Before = model.PlayerOptions.Before
            };

            // This if statement should go inside the ReplayEventsOptions somehow
            if (model.PlayerOptions.MaxDegreeOfParallelism.HasValue && model.PlayerOptions.MaxDegreeOfParallelism.Value > 0 && model.PlayerOptions.MaxDegreeOfParallelism.Value < 100)
                replayEventsOptions.MaxDegreeOfParallelism = model.PlayerOptions.MaxDegreeOfParallelism.Value;

            var command = new RebuildProjectionCommand(new ProjectionVersionManagerId(model.ProjectionContractId, contextAccessor.CronusContext.Tenant), model.Hash, replayEventsOptions);

            if (_publisher.Publish(command))
                return new OkObjectResult(new ResponseResult());

            return new BadRequestObjectResult(new ResponseResult<string>($"Unable to publish command '{nameof(ReplayProjection)}'"));
        }

        [HttpPost, Route("Replay")]
        public IActionResult Replay([FromBody] RequestModel model)
        {
            model.PlayerOptions ??= new PlayerOptions();
            var replayEventsOptions = new ReplayEventsOptions()
            {
                After = model.PlayerOptions.After,
                Before = model.PlayerOptions.Before
            };

            // This if statement should go inside the ReplayEventsOptions somehow
            if (model.PlayerOptions.MaxDegreeOfParallelism.HasValue && model.PlayerOptions.MaxDegreeOfParallelism.Value > 0 && model.PlayerOptions.MaxDegreeOfParallelism.Value < 100)
                replayEventsOptions.MaxDegreeOfParallelism = model.PlayerOptions.MaxDegreeOfParallelism.Value;

            var command = new ReplayProjection(new ProjectionVersionManagerId(model.ProjectionContractId, contextAccessor.CronusContext.Tenant), model.Hash, replayEventsOptions);

            if (_publisher.Publish(command))
                return new OkObjectResult(new ResponseResult());

            return new BadRequestObjectResult(new ResponseResult<string>($"Unable to publish command '{nameof(RebuildProjectionCommand)}'"));
        }

        public class RequestModel
        {
            public RequestModel()
            {
                PlayerOptions = new PlayerOptions();
            }

            [Required]
            public string ProjectionContractId { get; set; }

            [Required]
            public string Hash { get; set; }

            public PlayerOptions PlayerOptions { get; set; }
        }

        public class PlayerOptions
        {
            public DateTimeOffset? After { get; set; }

            public DateTimeOffset? Before { get; set; }

            public int? MaxDegreeOfParallelism { get; set; }
        }
    }
}
