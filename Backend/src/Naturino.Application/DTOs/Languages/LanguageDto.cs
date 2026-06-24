namespace Naturino.Application.DTOs.Languages;

public class LanguageDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NativeName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Locale { get; set; } = string.Empty;
    public string? Flag { get; set; }
    public string Direction { get; set; } = "ltr";
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class LanguageCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string NativeName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Locale { get; set; } = string.Empty;
    public string? Flag { get; set; }
    public string Direction { get; set; } = "ltr";
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}

public class LanguageUpdateDto : LanguageCreateDto
{
}

public class LanguageReorderItemDto
{
    public Guid Id { get; set; }
    public int SortOrder { get; set; }
}
