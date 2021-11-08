using Elders.Cronus.EventStore.Players;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Elders.Cronus.Api.Controllers
{
    public class ReplayPublicEventController : ApiControllerBase
    {
        private readonly IPublisher<ISystemEvent> signalPublisher;
        private DateTimeOffset ReplayAfterDefaultDate = new DateTimeOffset(2000, 1, 1, 0, 0, 0, 0, TimeSpan.FromHours(0));

        public ReplayPublicEventController(IPublisher<ISystemEvent> signalPublisher)
        {
            this.signalPublisher = signalPublisher;
        }

        [HttpPost, Route("ReplayPublicEvent")]
        public IActionResult ReplayPublicEvent([FromBody] ReplayPublicEventRequest model)
        {
            if (model.ReplayAfter.HasValue)
                ReplayAfterDefaultDate = model.ReplayAfter.Value;

            var replay = new ReplayPublicEventsRequested()
            {
                Tenant = model.Tenant,
                RecipientBoundedContext = model.RecipientBoundedContext,
                RecipientHandlers = model.RecipientHandlers,
                SourceEventTypeId = model.SourceEventTypeId,
                ReplayOptions = new ReplayPublicEventsOptions()
                {
                    After = ReplayAfterDefaultDate
                }
            };

            if (signalPublisher.Publish(replay))
                return new OkObjectResult(new ResponseResult());

            return new BadRequestObjectResult(new ResponseResult<string>($"Unable to publish '{nameof(ReplayPublicEventsRequested)}'"));
        }
    }

    public class ReplayPublicEventRequest
    {
        [Required]
        public string Tenant { get; set; }

        [Required]
        public string RecipientBoundedContext { get; set; }

        [Required]
        public string RecipientHandlers { get; set; }

        [Required]
        public string SourceEventTypeId { get; set; }

        public DateTimeOffset? ReplayAfter { get; set; }
    }
}
