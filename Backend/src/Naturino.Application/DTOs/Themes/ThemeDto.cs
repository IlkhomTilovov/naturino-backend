namespace Naturino.Application.DTOs.Themes;

public class ThemeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Version { get; set; } = "1.0.0";
    public bool IsActive { get; set; }
    public bool IsDarkMode { get; set; }
    public string AppearanceMode { get; set; } = "light";
    public string FontHeading { get; set; } = string.Empty;
    public string FontBody { get; set; } = string.Empty;
    public string ColorTokensJson { get; set; } = "{}";
    public string TypographyTokensJson { get; set; } = "{}";
    public string RadiusTokensJson { get; set; } = "{}";
    public string ShadowTokensJson { get; set; } = "{}";
    public string ButtonTokensJson { get; set; } = "{}";
    public string BrandingTokensJson { get; set; } = "{}";
    public string LayoutTokensJson { get; set; } = "{}";
    public string AnimationTokensJson { get; set; } = "{}";
    public string CustomCss { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class ThemeCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public string Version { get; set; } = "1.0.0";
    public string AppearanceMode { get; set; } = "light";
    public string FontHeading { get; set; } = "Inter";
    public string FontBody { get; set; } = "Inter";
    public string ColorTokensJson { get; set; } = "{}";
    public string TypographyTokensJson { get; set; } = "{}";
    public string RadiusTokensJson { get; set; } = "{}";
    public string ShadowTokensJson { get; set; } = "{}";
    public string ButtonTokensJson { get; set; } = "{}";
    public string BrandingTokensJson { get; set; } = "{}";
    public string LayoutTokensJson { get; set; } = "{}";
    public string AnimationTokensJson { get; set; } = "{}";
    public string CustomCss { get; set; } = "";
}

public class ThemeUpdateDto : ThemeCreateDto
{
}
