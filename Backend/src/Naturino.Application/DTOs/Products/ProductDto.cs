namespace Naturino.Application.DTOs.Products;

public class ProductImageDto
{
    public Guid Id { get; set; }
    public Guid MediaFileId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }
}

public class AddProductImagesDto
{
    public List<Guid> MediaFileIds { get; set; } = [];
}

public class ReorderProductImagesDto
{
    public List<Guid> ImageIds { get; set; } = [];
}

public class UpdateProductImageDto
{
    public string? AltText { get; set; }
}

public class ProductDto
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }
    public int StockQuantity { get; set; }
    public decimal? Weight { get; set; }
    public string? Brand { get; set; }
    public string? AgeGroup { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public List<ProductImageDto> Images { get; set; } = [];
}

public class ProductCreateDto
{
    public Guid CategoryId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }
    public int StockQuantity { get; set; }
    public decimal? Weight { get; set; }
    public string? Brand { get; set; }
    public string? AgeGroup { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; } = true;
    public List<Guid> MediaFileIds { get; set; } = [];
}

public class ProductUpdateDto : ProductCreateDto
{
}

public class ProductQueryDto : Common.PaginationQuery
{
    public Guid? CategoryId { get; set; }
    public bool? IsFeatured { get; set; }
}
