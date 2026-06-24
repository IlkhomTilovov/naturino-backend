using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naturino.Application.DTOs.Products;
using Naturino.Application.Services;

namespace Naturino.API.Controllers;

[ApiController]
[Route("api/v1/products/{productId:guid}/images")]
[Authorize]
public class ProductImagesController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductImagesController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid productId, CancellationToken ct)
    {
        return Ok(await _productService.GetImagesAsync(productId, ct));
    }

    [HttpPost]
    public async Task<IActionResult> Add(Guid productId, [FromBody] AddProductImagesDto dto, CancellationToken ct)
    {
        return Ok(await _productService.AddImagesAsync(productId, dto.MediaFileIds, ct));
    }

    [HttpDelete("{imageId:guid}")]
    public async Task<IActionResult> Remove(Guid productId, Guid imageId, CancellationToken ct)
    {
        await _productService.RemoveImageAsync(productId, imageId, ct);
        return NoContent();
    }

    [HttpPatch("{imageId:guid}/cover")]
    public async Task<IActionResult> SetCover(Guid productId, Guid imageId, CancellationToken ct)
    {
        await _productService.SetCoverImageAsync(productId, imageId, ct);
        return NoContent();
    }

    [HttpPatch("reorder")]
    public async Task<IActionResult> Reorder(Guid productId, [FromBody] ReorderProductImagesDto dto, CancellationToken ct)
    {
        await _productService.ReorderImagesAsync(productId, dto.ImageIds, ct);
        return NoContent();
    }

    [HttpPatch("{imageId:guid}")]
    public async Task<IActionResult> Update(Guid productId, Guid imageId, [FromBody] UpdateProductImageDto dto, CancellationToken ct)
    {
        return Ok(await _productService.UpdateImageAsync(productId, imageId, dto.AltText, ct));
    }
}
