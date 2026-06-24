using Microsoft.AspNetCore.Mvc;

namespace Naturino.API.Controllers;

[ApiController]
[Route("api/v1/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "Healthy",
            service = "Naturino.API",
            timestampUtc = DateTimeOffset.UtcNow
        });
    }
}
