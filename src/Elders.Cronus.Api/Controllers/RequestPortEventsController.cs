using Elders.Cronus.EventStore.Players;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Elders.Cronus.Api.Controllers
{
    public class RequestPortEventsController : ApiControllerBase
    {
        private readonly IPublisher<ISystemSignal> signalPublisher;
        private readonly TypeContainer<IPort> portsContainer;

        private DateTimeOffset ReplayAfterDefaultDate = new DateTimeOffset(2000, 1, 1, 0, 0, 0, 0, TimeSpan.FromHours(0));
        private DateTimeOffset ReplayBeforeDefaultDate = new DateTimeOffset(2100, 1, 1, 0, 0, 0, 0, TimeSpan.FromHours(0));

        public RequestPortEventsController(IPublisher<ISystemSignal> signalPublisher, TypeContainer<IPort> portsContainer)
        {
            this.signalPublisher = signalPublisher;
            this.portsContainer = portsContainer;
        }

        [HttpPost, Route("RequestPortEvents")]
        public IActionResult ReplayPublicEvent([FromBody] RequestPortEventsRequest model)
        {
            if (model.ReplayAfter.HasValue)
                ReplayAfterDefaultDate = model.ReplayAfter.Value;

            if (model.ReplayBefore.HasValue)
                ReplayBeforeDefaultDate = model.ReplayBefore.Value;

            Type port = portsContainer.Items.Where(p => p.GetContractId().Equals(model.RecipientHandler)).SingleOrDefault();
            var bc = port.GetBoundedContext();
            if (port is null)
                return new BadRequestObjectResult(new ResponseResult<string>($"Unable to find port {model.RecipientHandler}"));

            var portEvents = port
                .GetInterfaces()
                .Where(x => x.IsGenericType && (x.GetGenericTypeDefinition() == typeof(IEventHandler<>) || x.GetGenericTypeDefinition() == typeof(IPublicEventHandler<>)))
                .SelectMany(ff => ff.GetGenericArguments()).Where(x => typeof(IEvent).IsAssignableFrom(x) || typeof(IPublicEvent).IsAssignableFrom(x))
                .Select(x =>
                    new Something
                    {
                        EventBoundedContext = x.GetCustomAttribute<DataContractAttribute>().Namespace,
                        EventId = x.GetCustomAttribute<DataContractAttribute>().Name,
                    });

            foreach (var portEvent in portEvents)
            {
                var replay = new ReplayPublicEventsRequested()
                {
                    Tenant = model.Tenant,
                    RecipientBoundedContext = bc,
                    RecipientHandlers = model.RecipientHandler,
                    SourceEventTypeId = portEvent.EventId,
                    ReplayOptions = new ReplayEventsOptions()
                    {
                        After = ReplayAfterDefaultDate,
                        Before = ReplayBeforeDefaultDate
                    }
                };

                var headers = new Dictionary<string, string>();
                headers.Add(MessageHeader.BoundedContext, portEvent.EventBoundedContext);

                signalPublisher.Publish(replay, headers);
            }

            return Ok();
        }
    }

    class Something
    {
        public string EventBoundedContext { get; set; }
        public string EventId { get; set; }

    }

    public class RequestPortEventsRequest
    {
        [Required]
        public string Tenant { get; set; }

        [Required]
        public string RecipientHandler { get; set; } // The port id/name

        public DateTimeOffset? ReplayAfter { get; set; }

        public DateTimeOffset? ReplayBefore { get; set; }
    }
}
