using System.Web.Http;
using Elders.Cronus;
using Elders.Web.Api;
using static Elders.Cronus.Api.EventStoreExplorer;
using System.Web.Http.ModelBinding;

namespace Elders.Cronus.Api.Controllers
{
    [RoutePrefix("EventStore")]
    public class EventStoreController : ApiController
    {
        public EventStoreExplorer EventStoreExplorer { get; set; }

        [HttpGet, Route("Explore")]
        public ResponseResult<AggregateDto> Explore(RequestModel model)
        {
            AggregateDto result = EventStoreExplorer.Explore(model.Id);
            return new ResponseResult<AggregateDto>(result);
        }

        [ModelBinder(typeof(UrlBinder))]
        public class RequestModel
        {
            public StringTenantId Id { get; set; }
        }
    }
}
