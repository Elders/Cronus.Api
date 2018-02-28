using System.Web.Http;
using Elders.Cronus;
using Elders.Web.Api;
using System.Web.Http.ModelBinding;
using static Elders.Cronus.Api.ProjectionExplorer;

namespace Elders.Cronus.Api.Controllers
{
    [RoutePrefix("Projection")]
    public class ProjectionController : ApiController
    {
        public ProjectionExplorer ProjectionExplorer { get; set; }

        [HttpGet, Route("Explore")]
        public ResponseResult<ProjectionDto> Explore(RequestModel model)
        {
            var projectionType = model.ProjectionContractId.GetTypeByContract();
            ProjectionDto result = ProjectionExplorer.Explore(model.Id, projectionType);
            return new ResponseResult<ProjectionDto>(result);
        }

        [ModelBinder(typeof(UrlBinder))]
        public class RequestModel
        {
            public StringTenantId Id { get; set; }

            public string ProjectionContractId { get; set; }
        }
    }
}
