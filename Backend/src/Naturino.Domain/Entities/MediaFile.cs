using Naturino.Domain.Common;
using Naturino.Domain.Enums;

namespace Naturino.Domain.Entities;

public class MediaFile : ISoftDeletable
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? FolderId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public MediaSourceType SourceType { get; set; } = MediaSourceType.Local;
    public long FileSizeBytes { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public string? AltText { get; set; }
    public Guid? UploadedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public MediaFolder? Folder { get; set; }
    public User? UploadedByUser { get; set; }
}
