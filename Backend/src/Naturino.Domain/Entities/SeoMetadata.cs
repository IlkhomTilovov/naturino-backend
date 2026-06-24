using Naturino.Domain.Enums;

namespace Naturino.Domain.Entities;

public class SeoMetadata
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public SeoEntityType EntityType { get; set; }
    public Guid EntityId { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public string? OgTitle { get; set; }
    public string? OgDescription { get; set; }
    public Guid? OgImageFileId { get; set; }
    public string? CanonicalUrl { get; set; }
    public bool NoIndex { get; set; }

    /// <summary>JSON-LD override, stored as jsonb.</summary>
    public string? StructuredData { get; set; }

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public MediaFile? OgImageFile { get; set; }
}
