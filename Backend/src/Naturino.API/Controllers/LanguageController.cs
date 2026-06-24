using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naturino.Application.DTOs.Languages;
using Naturino.Application.Services;

namespace Naturino.API.Controllers;

[ApiController]
[Route("api/v1/languages")]
public class LanguageController : ControllerBase
{
    private readonly ILanguageService _languageService;

    public LanguageController(ILanguageService languageService)
    {
        _languageService = languageService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        return Ok(await _languageService.GetAllAsync(ct));
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        return Ok(await _languageService.GetByIdAsync(id, ct));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] LanguageCreateDto dto, CancellationToken ct)
    {
        var result = await _languageService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] LanguageUpdateDto dto, CancellationToken ct)
    {
        return Ok(await _languageService.UpdateAsync(id, dto, ct));
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _languageService.DeleteAsync(id, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/set-default")]
    [Authorize]
    public async Task<IActionResult> SetDefault(Guid id, CancellationToken ct)
    {
        return Ok(await _languageService.SetDefaultAsync(id, ct));
    }

    [HttpPatch("{id:guid}/toggle-status")]
    [Authorize]
    public async Task<IActionResult> ToggleStatus(Guid id, CancellationToken ct)
    {
        return Ok(await _languageService.ToggleStatusAsync(id, ct));
    }

    [HttpPatch("sort")]
    [Authorize]
    public async Task<IActionResult> Reorder([FromBody] List<LanguageReorderItemDto> items, CancellationToken ct)
    {
        await _languageService.ReorderAsync(items, ct);
        return NoContent();
    }
}
