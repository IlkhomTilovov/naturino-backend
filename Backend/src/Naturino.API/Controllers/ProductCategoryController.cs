using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naturino.Application.DTOs.Products;
using Naturino.Application.Services;

namespace Naturino.API.Controllers;

[ApiController]
[Route("api/v1/product-categories")]
public class ProductCategoryController : ControllerBase
{
    private readonly IProductCategoryService _categoryService;

    public ProductCategoryController(IProductCategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        return Ok(await _categoryService.GetAllAsync(ct));
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        return Ok(await _categoryService.GetByIdAsync(id, ct));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] ProductCategoryCreateDto dto, CancellationToken ct)
    {
        var result = await _categoryService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductCategoryUpdateDto dto, CancellationToken ct)
    {
        return Ok(await _categoryService.UpdateAsync(id, dto, ct));
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _categoryService.DeleteAsync(id, ct);
        return NoContent();
    }

    [HttpPut("reorder")]
    [Authorize]
    public async Task<IActionResult> Reorder([FromBody] List<ProductCategoryReorderItemDto> items, CancellationToken ct)
    {
        await _categoryService.ReorderAsync(items, ct);
        return NoContent();
    }
}
