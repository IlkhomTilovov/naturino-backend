using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naturino.Application.DTOs.Contacts;
using Naturino.Application.Services;

namespace Naturino.API.Controllers;

[ApiController]
[Route("api/v1/contacts")]
public class ContactController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Submit([FromBody] CreateContactDto dto, CancellationToken ct)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _contactService.SubmitAsync(dto, ip, ct);
        return Ok(result);
    }
}
