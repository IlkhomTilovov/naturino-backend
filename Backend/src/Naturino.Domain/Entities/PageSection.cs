using Naturino.Domain.Common;
using Naturino.Domain.Enums;

namespace Naturino.Domain.Entities;

public class PageSection : BaseEntity
{
    public Guid PageId { get; set; }
    public SectionType SectionType { get; set; }
    public int SortOrder { get; set; }
    public bool IsEnabled { get; set; } = true;

    /// <summary>Raw JSON stored as PostgreSQL jsonb; shape depends on <see cref="SectionType"/> and is validated in the Application layer. This is what the public site serves.</summary>
    public string Content { get; set; } = "{}";

    /// <summary>Pending unpublished edits, written by the admin editor's autosave. Null when there is nothing pending. Copied into <see cref="Content"/> on publish.</summary>
    public string? DraftContent { get; set; }

    public Page Page { get; set; } = null!;
}
