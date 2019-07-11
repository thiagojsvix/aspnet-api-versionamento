using Microsoft.AspNetCore.Mvc;

namespace Versionamento.WebApi.V1.Controllers
{
    [ApiVersionNeutral]
    [Route("api/[controller]")]
    public class StatusCodeController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetStatusCode() => this.Ok();
    }
}
