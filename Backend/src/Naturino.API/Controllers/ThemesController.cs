using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naturino.Application.DTOs.Themes;
using Naturino.Application.Services;

namespace Naturino.API.Controllers;

[ApiController]
[Route("api/v1/themes")]
public class ThemesController : ControllerBase
{
    private readonly IThemeService _themeService;

    public ThemesController(IThemeService themeService)
    {
        _themeService = themeService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        return Ok(await _themeService.GetAllAsync(ct));
    }

    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<IActionResult> GetActive(CancellationToken ct)
    {
        return Ok(await _themeService.GetActiveAsync(ct));
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        return Ok(await _themeService.GetByIdAsync(id, ct));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] ThemeCreateDto dto, CancellationToken ct)
    {
        var result = await _themeService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] ThemeUpdateDto dto, CancellationToken ct)
    {
        return Ok(await _themeService.UpdateAsync(id, dto, ct));
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _themeService.DeleteAsync(id, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/duplicate")]
    [Authorize]
    public async Task<IActionResult> Duplicate(Guid id, CancellationToken ct)
    {
        return Ok(await _themeService.DuplicateAsync(id, ct));
    }

    [HttpPatch("{id:guid}/activate")]
    [Authorize]
    public async Task<IActionResult> Activate(Guid id, CancellationToken ct)
    {
        return Ok(await _themeService.SetActiveAsync(id, ct));
    }
}
