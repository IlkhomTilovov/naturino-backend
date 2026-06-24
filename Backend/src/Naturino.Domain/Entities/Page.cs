using Naturino.Domain.Common;

namespace Naturino.Domain.Entities;

public class Page : BaseEntity, ISoftDeletable, IAuditable
{
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool IsHomePage { get; set; }
    public bool IsPublished { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public Guid? OgImageFileId { get; set; }
    public bool IsIndexable { get; set; } = true;
    public bool IsFollow { get; set; } = true;

    public MediaFile? OgImageFile { get; set; }
    public ICollection<PageSection> Sections { get; set; } = new List<PageSection>();
}
