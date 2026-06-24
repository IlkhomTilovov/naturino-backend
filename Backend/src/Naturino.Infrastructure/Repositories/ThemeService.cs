using Microsoft.EntityFrameworkCore;
using Naturino.Application.DTOs.Themes;
using Naturino.Application.Services;
using Naturino.Domain.Entities;
using Naturino.Domain.Exceptions;
using Naturino.Infrastructure.Common;
using Naturino.Infrastructure.Persistence;

namespace Naturino.Infrastructure.Repositories;

public class ThemeService : IThemeService
{
    private readonly ApplicationDbContext _context;

    public ThemeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ThemeDto>> GetAllAsync(CancellationToken ct = default)
    {
        var themes = await _context.Themes.OrderByDescending(t => t.CreatedAt).ToListAsync(ct);
        return themes.Select(ToDto).ToList();
    }

    public async Task<ThemeDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return ToDto(await FindAsync(id, ct));
    }

    public async Task<ThemeDto> GetActiveAsync(CancellationToken ct = default)
    {
        var theme = await _context.Themes.FirstOrDefaultAsync(t => t.IsActive, ct)
            ?? throw new NotFoundException(nameof(Theme), "active");
        return ToDto(theme);
    }

    public async Task<ThemeDto> CreateAsync(ThemeCreateDto dto, CancellationToken ct = default)
    {
        Validate(dto);

        var slug = string.IsNullOrWhiteSpace(dto.Slug) ? SlugHelper.Generate(dto.Name) : SlugHelper.Generate(dto.Slug);
        await EnsureUniqueSlugAsync(slug, null, ct);

        var theme = new Theme
        {
            Name = dto.Name.Trim(),
            Slug = slug,
            Description = dto.Description,
            Version = dto.Version,
            AppearanceMode = dto.AppearanceMode,
            IsDarkMode = dto.AppearanceMode == "dark",
            FontHeading = dto.FontHeading,
            FontBody = dto.FontBody,
            ColorTokensJson = dto.ColorTokensJson,
            TypographyTokensJson = dto.TypographyTokensJson,
            RadiusTokensJson = dto.RadiusTokensJson,
            ShadowTokensJson = dto.ShadowTokensJson,
            ButtonTokensJson = dto.ButtonTokensJson,
            BrandingTokensJson = dto.BrandingTokensJson,
            LayoutTokensJson = dto.LayoutTokensJson,
            AnimationTokensJson = dto.AnimationTokensJson,
            CustomCss = dto.CustomCss,
            IsActive = !await _context.Themes.AnyAsync(ct),
        };

        _context.Themes.Add(theme);
        await _context.SaveChangesAsync(ct);
        return ToDto(theme);
    }

    public async Task<ThemeDto> UpdateAsync(Guid id, ThemeUpdateDto dto, CancellationToken ct = default)
    {
        Validate(dto);
        var theme = await FindAsync(id, ct);

        var slug = string.IsNullOrWhiteSpace(dto.Slug) ? SlugHelper.Generate(dto.Name) : SlugHelper.Generate(dto.Slug);
        await EnsureUniqueSlugAsync(slug, id, ct);

        theme.Name = dto.Name.Trim();
        theme.Slug = slug;
        theme.Description = dto.Description;
        theme.Version = dto.Version;
        theme.AppearanceMode = dto.AppearanceMode;
        theme.IsDarkMode = dto.AppearanceMode == "dark";
        theme.FontHeading = dto.FontHeading;
        theme.FontBody = dto.FontBody;
        theme.ColorTokensJson = dto.ColorTokensJson;
        theme.TypographyTokensJson = dto.TypographyTokensJson;
        theme.RadiusTokensJson = dto.RadiusTokensJson;
        theme.ShadowTokensJson = dto.ShadowTokensJson;
        theme.ButtonTokensJson = dto.ButtonTokensJson;
        theme.BrandingTokensJson = dto.BrandingTokensJson;
        theme.LayoutTokensJson = dto.LayoutTokensJson;
        theme.AnimationTokensJson = dto.AnimationTokensJson;
        theme.CustomCss = dto.CustomCss;
        theme.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(ct);
        return ToDto(theme);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var theme = await FindAsync(id, ct);
        if (theme.IsActive)
        {
            throw new ConflictException("Faol mavzuni o'chirib bo'lmaydi.");
        }

        _context.Themes.Remove(theme);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<ThemeDto> DuplicateAsync(Guid id, CancellationToken ct = default)
    {
        var source = await FindAsync(id, ct);

        var baseName = $"{source.Name} (nusxa)";
        var baseSlug = SlugHelper.Generate(baseName);
        var slug = baseSlug;
        var suffix = 1;
        while (await _context.Themes.AnyAsync(t => t.Slug == slug, ct))
        {
            slug = $"{baseSlug}-{++suffix}";
        }

        var copy = new Theme
        {
            Name = baseName,
            Slug = slug,
            Description = source.Description,
            Version = source.Version,
            AppearanceMode = source.AppearanceMode,
            IsActive = false,
            IsDarkMode = source.IsDarkMode,
            FontHeading = source.FontHeading,
            FontBody = source.FontBody,
            ColorTokensJson = source.ColorTokensJson,
            TypographyTokensJson = source.TypographyTokensJson,
            RadiusTokensJson = source.RadiusTokensJson,
            ShadowTokensJson = source.ShadowTokensJson,
            ButtonTokensJson = source.ButtonTokensJson,
            BrandingTokensJson = source.BrandingTokensJson,
            LayoutTokensJson = source.LayoutTokensJson,
            AnimationTokensJson = source.AnimationTokensJson,
            CustomCss = source.CustomCss,
        };

        _context.Themes.Add(copy);
        await _context.SaveChangesAsync(ct);
        return ToDto(copy);
    }

    public async Task<ThemeDto> SetActiveAsync(Guid id, CancellationToken ct = default)
    {
        var theme = await FindAsync(id, ct);

        var currentActive = await _context.Themes.Where(t => t.IsActive && t.Id != id).ToListAsync(ct);
        foreach (var other in currentActive)
        {
            other.IsActive = false;
            other.UpdatedAt = DateTimeOffset.UtcNow;
        }

        theme.IsActive = true;
        theme.UpdatedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync(ct);
        return ToDto(theme);
    }

    private static void Validate(ThemeCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["name"] = ["Mavzu nomi majburiy."],
            });
        }
    }

    private async Task EnsureUniqueSlugAsync(string slug, Guid? excludeId, CancellationToken ct)
    {
        var exists = await _context.Themes.AnyAsync(t => t.Slug == slug && t.Id != excludeId, ct);
        if (exists)
        {
            throw new ConflictException($"\"{slug}\" slug bilan mavzu allaqachon mavjud.");
        }
    }

    private async Task<Theme> FindAsync(Guid id, CancellationToken ct)
    {
        return await _context.Themes.FirstOrDefaultAsync(t => t.Id == id, ct)
            ?? throw new NotFoundException(nameof(Theme), id);
    }

    private static ThemeDto ToDto(Theme theme) => new()
    {
        Id = theme.Id,
        Name = theme.Name,
        Slug = theme.Slug,
        Description = theme.Description,
        Version = theme.Version,
        IsActive = theme.IsActive,
        IsDarkMode = theme.IsDarkMode,
        AppearanceMode = theme.AppearanceMode,
        FontHeading = theme.FontHeading,
        FontBody = theme.FontBody,
        ColorTokensJson = theme.ColorTokensJson,
        TypographyTokensJson = theme.TypographyTokensJson,
        RadiusTokensJson = theme.RadiusTokensJson,
        ShadowTokensJson = theme.ShadowTokensJson,
        ButtonTokensJson = theme.ButtonTokensJson,
        BrandingTokensJson = theme.BrandingTokensJson,
        LayoutTokensJson = theme.LayoutTokensJson,
        AnimationTokensJson = theme.AnimationTokensJson,
        CustomCss = theme.CustomCss,
        CreatedAt = theme.CreatedAt,
        UpdatedAt = theme.UpdatedAt,
    };
}
