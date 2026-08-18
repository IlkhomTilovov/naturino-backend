using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naturino.Application.DTOs.Shops;
using Naturino.Application.Services;

namespace Naturino.API.Controllers;

[ApiController]
[Route("api/v1/shops")]
public class ShopController : ControllerBase
{
    private readonly IShopService _shopService;

    public ShopController(IShopService shopService)
    {
        _shopService = shopService;
    }

    // Unpaginated, active-only — the public "Qayerdan sotib olish" directory
    // groups these client-side by country/city, so it needs the full set in
    // one request rather than a page at a time.
    [HttpGet("public")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllActive(CancellationToken ct)
    {
        return Ok(await _shopService.GetAllActiveAsync(ct));
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPaged([FromQuery] ShopQueryDto query, CancellationToken ct)
    {
        return Ok(await _shopService.GetPagedAsync(query, ct));
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        return Ok(await _shopService.GetByIdAsync(id, ct));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] ShopCreateDto dto, CancellationToken ct)
    {
        var result = await _shopService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] ShopUpdateDto dto, CancellationToken ct)
    {
        return Ok(await _shopService.UpdateAsync(id, dto, ct));
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _shopService.DeleteAsync(id, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/toggle-status")]
    [Authorize]
    public async Task<IActionResult> ToggleStatus(Guid id, CancellationToken ct)
    {
        return Ok(await _shopService.ToggleStatusAsync(id, ct));
    }
}
