using System.Web.Http;
using Elders.Cronus.Projections.Versioning;
using Elders.Web.Api;

namespace Elders.Cronus.Api.Controllers
{
    public class ProjectionRebuildController : ApiController
    {
        public IPublisher<ICommand> Publisher { get; set; }

        [HttpPost]
        public IHttpActionResult Rebuild(RequestModel model)
        {
            var command = new RebuildProjection(new ProjectionVersionManagerId(model.ProjectionContractId), model.Hash);

            if (Publisher.Publish(command))
                return this.Accepted(new ResponseResult());

            return this.NotAcceptable(new ResponseResult($"Unable to publish command '{nameof(RebuildProjection)}'"));
        }

        public class RequestModel
        {
            public string ProjectionContractId { get; set; }

            public string Hash { get; set; }
        }
    }
}
