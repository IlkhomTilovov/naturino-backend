namespace Naturino.Application.DTOs.Certificates;

public class CertificateDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IssuedBy { get; set; }
    public string? CertificateNumber { get; set; }
    public DateOnly? IssueDate { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public string? Icon { get; set; }
    public string? Category { get; set; }
    public string? Scope { get; set; }
    public string? VerificationUrl { get; set; }
    public Dictionary<string, CertificateTranslationDto> Translations { get; set; } = [];
    public Guid? FileId { get; set; }
    public string? FileUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class CertificateTranslationDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
}

public class CertificateCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IssuedBy { get; set; }
    public string? CertificateNumber { get; set; }
    public DateOnly? IssueDate { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public string? Icon { get; set; }
    public string? Category { get; set; }
    public string? Scope { get; set; }
    public string? VerificationUrl { get; set; }
    public Dictionary<string, CertificateTranslationDto> Translations { get; set; } = [];
    public Guid? FileId { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class CertificateUpdateDto : CertificateCreateDto { }

public class CertificateReorderItemDto
{
    public Guid Id { get; set; }
    public int SortOrder { get; set; }
}
