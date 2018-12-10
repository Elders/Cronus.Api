using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using static Elders.Cronus.Api.EventStoreExplorer;
using System.ComponentModel.DataAnnotations;

namespace Elders.Cronus.Api.Controllers
{
    [Route("EventStore")]
    public class EventStoreController : ApiControllerBase
    {
        private readonly EventStoreExplorer _eventExplorer;

        public EventStoreController(EventStoreExplorer eventStoreExplorer)
        {
            if (eventStoreExplorer is null) throw new ArgumentNullException(nameof(eventStoreExplorer));

            _eventExplorer = eventStoreExplorer;
        }

        [HttpGet, Route("Explore")]
        public async Task<IActionResult> Explore([FromQuery]RequestModel model)
        {
            AggregateDto result = await _eventExplorer.ExploreAsync(model.Id.ToStringTenantId());
            return new OkObjectResult(new ResponseResult<AggregateDto>(result));
        }

        public class RequestModel
        {
            [Required]
            public string Id { get; set; }
        }
    }
}
