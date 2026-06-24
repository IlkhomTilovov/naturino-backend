using Naturino.Domain.Common;

namespace Naturino.Domain.Entities;

public class Blog : BaseEntity, ISoftDeletable
{
    public Guid? CategoryId { get; set; }
    public Guid? AuthorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Excerpt { get; set; }
    public string? Content { get; set; }
    public Guid? FeaturedImageId { get; set; }
    public bool IsPublished { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public int ViewCount { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public BlogCategory? Category { get; set; }
    public User? Author { get; set; }
    public MediaFile? FeaturedImage { get; set; }
}
