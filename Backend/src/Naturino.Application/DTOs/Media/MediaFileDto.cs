using Naturino.Domain.Enums;

namespace Naturino.Application.DTOs.Media;

public class MediaFileDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public MediaSourceType SourceType { get; set; }
    public long FileSizeBytes { get; set; }
    public string? AltText { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class CreateMediaFromUrlDto
{
    public string Url { get; set; } = string.Empty;
    public string? AltText { get; set; }
}
