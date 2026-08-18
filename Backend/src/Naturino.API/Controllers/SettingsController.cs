using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naturino.Application.Services;

namespace Naturino.API.Controllers;

[ApiController]
[Route("api/v1/settings")]
public class SettingsController : ControllerBase
{
    private readonly ISettingService _settingService;

    public SettingsController(ISettingService settingService)
    {
        _settingService = settingService;
    }

    [HttpGet("{groupName}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetGroup(string groupName, CancellationToken ct)
    {
        return Ok(await _settingService.GetGroupAsync(groupName, ct));
    }

    [HttpPut("{groupName}")]
    [Authorize]
    public async Task<IActionResult> UpdateGroup(string groupName, [FromBody] Dictionary<string, string> values, CancellationToken ct)
    {
        return Ok(await _settingService.UpdateGroupAsync(groupName, values, ct));
    }
}
