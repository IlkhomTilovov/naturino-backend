using Naturino.Domain.Common;

namespace Naturino.Domain.Entities;

public class Certificate : BaseEntity, ISoftDeletable
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IssuedBy { get; set; }
    public string? CertificateNumber { get; set; }
    public DateOnly? IssueDate { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public Guid? FileId { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public MediaFile? File { get; set; }
}
