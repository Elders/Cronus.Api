using Microsoft.AspNetCore.Mvc;
using Elders.Cronus.Api.Core;
using static Elders.Cronus.Api.Core.EventStoreExplorer;
using System.Threading.Tasks;

namespace Elders.Cronus.Api.Core.Controllers
{
    [Route("EventStore")]
    public class EventStoreController : ControllerBase
    {
        public EventStoreExplorer EventStoreExplorer { get; set; }

        [HttpGet, Route("Explore")]
        public async Task<IActionResult> Explore(RequestModel model)
        {
            AggregateDto result = await EventStoreExplorer.ExploreAsync(model.Id);
            return new OkObjectResult(new ResponseResult<AggregateDto>(result));
        }

        public class RequestModel
        {
            public StringTenantId Id { get; set; }
        }
    }

    public class ResponseResult<T>
    {
        public ResponseResult(T result)
        {

        }
    }
}
