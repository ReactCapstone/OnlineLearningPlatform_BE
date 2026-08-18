using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace NVLearnHub.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DebugController : ControllerBase
    {
        [HttpGet("me")]
        public ActionResult<object> Me()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(new { IsAuthenticated = User.Identity?.IsAuthenticated, claims });
        }
    }
}
