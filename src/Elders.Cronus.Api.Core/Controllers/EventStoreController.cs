using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using static Elders.Cronus.Api.Core.EventStoreExplorer;

namespace Elders.Cronus.Api.Core.Controllers
{
    [Route("EventStore")]
    public class EventStoreController : ControllerBase
    {
        private readonly EventStoreExplorer _eventExplorer;

        public EventStoreController(EventStoreExplorer eventStoreExplorer)
        {
            if (eventStoreExplorer is null) throw new ArgumentNullException(nameof(eventStoreExplorer));

            _eventExplorer = eventStoreExplorer;
        }

        [HttpGet, Route("Explore")]
        public async Task<IActionResult> Explore(RequestModel model)
        {
            AggregateDto result = await _eventExplorer.ExploreAsync(model.Id);
            return new OkObjectResult(new ResponseResult<AggregateDto>(result));
        }

        public class RequestModel
        {
            public StringTenantId Id { get; set; }
        }
    }
}
