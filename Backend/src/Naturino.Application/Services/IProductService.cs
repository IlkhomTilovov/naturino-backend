using Naturino.Application.Common;
using Naturino.Application.DTOs.Products;

namespace Naturino.Application.Services;

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetPagedAsync(ProductQueryDto query, CancellationToken ct = default);
    Task<ProductDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ProductDto> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<List<ProductDto>> GetRelatedAsync(Guid productId, int take, CancellationToken ct = default);
    Task<ProductDto> CreateAsync(ProductCreateDto dto, Guid? userId, CancellationToken ct = default);
    Task<ProductDto> UpdateAsync(Guid id, ProductUpdateDto dto, Guid? userId, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task SetActiveAsync(Guid id, bool isActive, CancellationToken ct = default);
    Task<List<ProductImageDto>> GetImagesAsync(Guid productId, CancellationToken ct = default);
    Task<List<ProductImageDto>> AddImagesAsync(Guid productId, List<Guid> mediaFileIds, CancellationToken ct = default);
    Task RemoveImageAsync(Guid productId, Guid imageId, CancellationToken ct = default);
    Task SetCoverImageAsync(Guid productId, Guid imageId, CancellationToken ct = default);
    Task ReorderImagesAsync(Guid productId, List<Guid> orderedImageIds, CancellationToken ct = default);
    Task<ProductImageDto> UpdateImageAsync(Guid productId, Guid imageId, string? altText, CancellationToken ct = default);
}

public interface IProductCategoryService
{
    Task<List<ProductCategoryDto>> GetAllAsync(CancellationToken ct = default);
    Task<ProductCategoryDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ProductCategoryDto> CreateAsync(ProductCategoryCreateDto dto, CancellationToken ct = default);
    Task<ProductCategoryDto> UpdateAsync(Guid id, ProductCategoryUpdateDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task ReorderAsync(List<ProductCategoryReorderItemDto> items, CancellationToken ct = default);
}
