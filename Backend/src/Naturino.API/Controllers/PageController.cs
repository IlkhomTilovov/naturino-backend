using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naturino.Application.DTOs.Pages;
using Naturino.Application.Services;

namespace Naturino.API.Controllers;

[ApiController]
[Route("api/v1/pages")]
public class PageController : ControllerBase
{
    private readonly IPageService _pageService;

    public PageController(IPageService pageService)
    {
        _pageService = pageService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        return Ok(await _pageService.GetAllAsync(ct));
    }

    [HttpGet("by-slug/{slug?}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBySlug(string? slug, CancellationToken ct)
    {
        return Ok(await _pageService.GetBySlugAsync(slug ?? "", ct));
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        return Ok(await _pageService.GetByIdAsync(id, ct));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreatePageDto dto, CancellationToken ct)
    {
        var userId = GetUserId();
        var result = await _pageService.CreateAsync(dto, userId, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePageDto dto, CancellationToken ct)
    {
        var userId = GetUserId();
        return Ok(await _pageService.UpdateAsync(id, dto, userId, ct));
    }

    [HttpPatch("{id:guid}/publish")]
    [Authorize]
    public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
    {
        var userId = GetUserId();
        return Ok(await _pageService.PublishAsync(id, userId, ct));
    }

    [HttpPut("sections/{sectionId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateSection(Guid sectionId, [FromBody] UpdatePageSectionDto dto, CancellationToken ct)
    {
        return Ok(await _pageService.UpdateSectionAsync(sectionId, dto, ct));
    }

    [HttpPost("{pageId:guid}/sections")]
    [Authorize]
    public async Task<IActionResult> AddSection(Guid pageId, [FromBody] CreatePageSectionDto dto, CancellationToken ct)
    {
        var result = await _pageService.AddSectionAsync(pageId, dto, ct);
        return Ok(result);
    }

    [HttpDelete("sections/{sectionId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteSection(Guid sectionId, CancellationToken ct)
    {
        await _pageService.DeleteSectionAsync(sectionId, ct);
        return NoContent();
    }

    private Guid? GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(claim, out var id) ? id : null;
    }
}
