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

    /// <summary>JSON stored as jsonb: per-language overrides for Name/ShortDescription/Description,
    /// e.g. {"ru":{"name":"...","shortDescription":"...","description":"..."},"en":{...}}.
    /// The base Name/ShortDescription/Description columns above remain the default (uz) content.</summary>
    public string? Translations { get; set; }
    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }
    public int StockQuantity { get; set; }
    public decimal? Weight { get; set; }
    public string? Brand { get; set; }
    public string? AgeGroup { get; set; }

    /// <summary>JSON array stored as jsonb: [{ "label": "Protein", "value": "22", "unit": "%" }].</summary>
    public string? NutritionalInfo { get; set; }

    /// <summary>JSON array stored as jsonb: [{ "weight": "1.5", "unit": "kg", "barcode": "...", "isDefault": true }].</summary>
    public string? PackagingOptions { get; set; }

    /// <summary>JSON array stored as jsonb: [{ "name": "Real Salmon", "percentage": "32" }], ordered by array position.</summary>
    public string? IngredientsList { get; set; }

    /// <summary>JSON array stored as jsonb: [{ "code": "ISO22000", "certificateNumber": "...", "expiryDate": "..." }].</summary>
    public string? Certifications { get; set; }

    /// <summary>JSON object stored as jsonb: { "moq": "...", "hsCode": "...", "incoterms": ["FOB"], "exportMarkets": [...] }.</summary>
    public string? ExportInfo { get; set; }

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
