using Naturino.Domain.Common;

namespace Naturino.Domain.Entities;

public class Language : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string NativeName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Locale { get; set; } = string.Empty;
    public string? Flag { get; set; }
    public string Direction { get; set; } = "ltr";
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}
