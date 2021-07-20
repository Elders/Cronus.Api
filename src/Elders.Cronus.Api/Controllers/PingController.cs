using Microsoft.AspNetCore.Mvc;

namespace Elders.Cronus.Api.Controllers
{
    [Route("ping")]
    public class PingController : ApiControllerBase
    {
        [HttpGet]
        public IActionResult Ping()
        {
            return new OkObjectResult("pong");
        }
    }
}
