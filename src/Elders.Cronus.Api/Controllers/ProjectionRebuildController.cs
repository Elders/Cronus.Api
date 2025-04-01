using System;
using System.ComponentModel.DataAnnotations;
using Elders.Cronus.EventStore.Players;
using Elders.Cronus.MessageProcessing;
using Elders.Cronus.Projections.Versioning;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost, Route("Fix"), Route("Rebuild")]
        public IActionResult Fix([FromBody] RequestModel model)
        {
            model.PlayerOptions ??= new PlayerOptions();
            var replayEventsOptions = new ReplayEventsOptions()
            {
                After = model.PlayerOptions.After,
                Before = model.PlayerOptions.Before
            };

            // This if statement should go inside the ReplayEventsOptions somehow
            if (model.PlayerOptions.MaxDegreeOfParallelism.HasValue)
                replayEventsOptions.MaxDegreeOfParallelism = model.PlayerOptions.MaxDegreeOfParallelism.Value;

            var command = new FixProjectionVersion(new ProjectionVersionManagerId(model.ProjectionContractId, contextAccessor.CronusContext.Tenant), model.Hash, replayEventsOptions);

            if (_publisher.Publish(command))
                return new OkObjectResult(new ResponseResult());

            return new BadRequestObjectResult(new ResponseResult<string>($"Unable to publish command '{nameof(FixProjectionVersion)}'"));
        }

        [HttpPost, Route("New"), Route("Replay")]
        public IActionResult New([FromBody] RequestModel model)
        {
            model.PlayerOptions ??= new PlayerOptions();
            var replayEventsOptions = new ReplayEventsOptions()
            {
                After = model.PlayerOptions.After,
                Before = model.PlayerOptions.Before
            };

            // This if statement should go inside the ReplayEventsOptions somehow
            if (model.PlayerOptions.MaxDegreeOfParallelism.HasValue)
                replayEventsOptions.MaxDegreeOfParallelism = model.PlayerOptions.MaxDegreeOfParallelism.Value;

            var command = new NewProjectionVersion(new ProjectionVersionManagerId(model.ProjectionContractId, contextAccessor.CronusContext.Tenant), model.Hash, replayEventsOptions);

            if (_publisher.Publish(command))
                return new OkObjectResult(new ResponseResult());

            return new BadRequestObjectResult(new ResponseResult<string>($"Unable to publish command '{nameof(NewProjectionVersion)}'"));
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

            [Range(1, 10_000)]
            public int? MaxDegreeOfParallelism { get; set; }
        }
    }
}
