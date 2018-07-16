using System.Web.Http;
using Elders.Cronus.Projections;
using Elders.Cronus.Projections.Versioning;
using Elders.Web.Api;

namespace Elders.Cronus.Api.Controllers
{
    public class ProjectionCancelController : ApiController
    {
        public IPublisher<ICommand> Publisher { get; set; }

        [HttpPost]
        public IHttpActionResult Cancel(RequestModel model)
        {
            var version = new Projections.ProjectionVersion(model.ProjectionContractId, ProjectionStatus.Create(model.Version.Status), model.Version.Revision, model.Version.Hash);
            var command = new CancelProjectionVersionRequest(new ProjectionVersionManagerId(model.ProjectionContractId), version, model.Reason ?? "Canceled by user");

            if (Publisher.Publish(command))
                return this.Accepted(new ResponseResult());

            return this.NotAcceptable(new ResponseResult($"Unable to publish command '{nameof(CancelProjectionVersionRequest)}'"));
        }

        public class RequestModel
        {
            public string ProjectionContractId { get; set; }

            public ProjectionVersion Version { get; set; }

            public string Reason { get; set; }
        }
    }
}
