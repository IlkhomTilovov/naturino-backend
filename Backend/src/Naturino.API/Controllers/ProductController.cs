using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naturino.Application.DTOs.Products;
using Naturino.Application.Services;

namespace Naturino.API.Controllers;

[ApiController]
[Route("api/v1/products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetPaged([FromQuery] ProductQueryDto query, CancellationToken ct)
    {
        return Ok(await _productService.GetPagedAsync(query, ct));
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        return Ok(await _productService.GetByIdAsync(id, ct));
    }

    [HttpGet("by-slug/{slug}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken ct)
    {
        return Ok(await _productService.GetBySlugAsync(slug, ct));
    }

    [HttpGet("{id:guid}/related")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRelated(Guid id, [FromQuery] int take = 4, CancellationToken ct = default)
    {
        return Ok(await _productService.GetRelatedAsync(id, take, ct));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] ProductCreateDto dto, CancellationToken ct)
    {
        var userId = GetUserId();
        var result = await _productService.CreateAsync(dto, userId, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductUpdateDto dto, CancellationToken ct)
    {
        var userId = GetUserId();
        return Ok(await _productService.UpdateAsync(id, dto, userId, ct));
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _productService.DeleteAsync(id, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/active")]
    [Authorize]
    public async Task<IActionResult> SetActive(Guid id, [FromBody] bool isActive, CancellationToken ct)
    {
        await _productService.SetActiveAsync(id, isActive, ct);
        return NoContent();
    }

    private Guid? GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(claim, out var id) ? id : null;
    }
}
