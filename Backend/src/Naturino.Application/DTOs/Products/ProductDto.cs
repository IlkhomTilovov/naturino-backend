namespace Naturino.Application.DTOs.Products;

public class ProductTranslationDto
{
    public string? Name { get; set; }
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
}

public class NutritionalItemDto
{
    public string Label { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string? Unit { get; set; }
}

public class PackagingOptionDto
{
    public string? Weight { get; set; }
    public string? Unit { get; set; }
    public string? Barcode { get; set; }
    public bool IsDefault { get; set; }
}

public class IngredientItemDto
{
    public string Name { get; set; } = string.Empty;
    public string? Percentage { get; set; }
}

public class ProductCertificationDto
{
    public string Code { get; set; } = string.Empty;
    public string? CertificateNumber { get; set; }
    public string? ExpiryDate { get; set; }
}

public class ExportInfoDto
{
    public string? Moq { get; set; }
    public string? HsCode { get; set; }
    public List<string> Incoterms { get; set; } = [];
    public string? ProductionCapacity { get; set; }
    public string? LeadTime { get; set; }
    public List<string> ExportMarkets { get; set; } = [];
}

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
    public Dictionary<string, ProductTranslationDto> Translations { get; set; } = [];
    public List<NutritionalItemDto> NutritionalInfo { get; set; } = [];
    public List<PackagingOptionDto> PackagingOptions { get; set; } = [];
    public List<IngredientItemDto> IngredientsList { get; set; } = [];
    public List<ProductCertificationDto> Certifications { get; set; } = [];
    public ExportInfoDto ExportInfo { get; set; } = new();
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
    public Dictionary<string, ProductTranslationDto> Translations { get; set; } = [];
    public List<NutritionalItemDto> NutritionalInfo { get; set; } = [];
    public List<PackagingOptionDto> PackagingOptions { get; set; } = [];
    public List<IngredientItemDto> IngredientsList { get; set; } = [];
    public List<ProductCertificationDto> Certifications { get; set; } = [];
    public ExportInfoDto? ExportInfo { get; set; }
}

public class ProductUpdateDto : ProductCreateDto
{
}

public class ProductQueryDto : Common.PaginationQuery
{
    public Guid? CategoryId { get; set; }
    public bool? IsFeatured { get; set; }
}
