using Naturino.Domain.Common;

namespace Naturino.Domain.Entities;

public class Product : BaseEntity, ISoftDeletable, IAuditable
{
    public Guid CategoryId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public string? Ingredients { get; set; }
    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }
    public int StockQuantity { get; set; }
    public decimal? Weight { get; set; }
    public string? Brand { get; set; }
    public string? AgeGroup { get; set; }

    /// <summary>JSON stored as jsonb, e.g. protein %, fat %, fiber %.</summary>
    public string? NutritionalInfo { get; set; }

    /// <summary>JSON array stored as jsonb, e.g. [{ "weightKg": 1.5, "sku": "..." }].</summary>
    public string? PackagingOptions { get; set; }

    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public int SortOrder { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    public ProductCategory Category { get; set; } = null!;
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
}
