using Microsoft.EntityFrameworkCore;
using Naturino.Application.DTOs.Languages;
using Naturino.Application.Services;
using Naturino.Domain.Entities;
using Naturino.Domain.Exceptions;
using Naturino.Infrastructure.Persistence;

namespace Naturino.Infrastructure.Repositories;

public class LanguageService : ILanguageService
{
    private readonly ApplicationDbContext _context;

    public LanguageService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<LanguageDto>> GetAllAsync(CancellationToken ct = default)
    {
        var languages = await _context.Languages.OrderBy(l => l.SortOrder).ToListAsync(ct);
        return languages.Select(ToDto).ToList();
    }

    public async Task<LanguageDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var language = await FindAsync(id, ct);
        return ToDto(language);
    }

    public async Task<LanguageDto> CreateAsync(LanguageCreateDto dto, CancellationToken ct = default)
    {
        Validate(dto);
        await EnsureUniqueAsync(dto.Code, dto.Locale, null, ct);

        var isFirstLanguage = !await _context.Languages.AnyAsync(ct);

        var language = new Language
        {
            Name = dto.Name.Trim(),
            NativeName = dto.NativeName.Trim(),
            Code = dto.Code.Trim().ToLowerInvariant(),
            Locale = dto.Locale.Trim(),
            Flag = dto.Flag,
            Direction = dto.Direction == "rtl" ? "rtl" : "ltr",
            IsActive = dto.IsActive,
            SortOrder = dto.SortOrder,
            // The very first language created always becomes the default — there must
            // always be exactly one, and an empty list has no other candidate.
            IsDefault = isFirstLanguage,
        };

        _context.Languages.Add(language);
        await _context.SaveChangesAsync(ct);
        return ToDto(language);
    }

    public async Task<LanguageDto> UpdateAsync(Guid id, LanguageUpdateDto dto, CancellationToken ct = default)
    {
        Validate(dto);
        var language = await FindAsync(id, ct);
        await EnsureUniqueAsync(dto.Code, dto.Locale, id, ct);

        language.Name = dto.Name.Trim();
        language.NativeName = dto.NativeName.Trim();
        language.Code = dto.Code.Trim().ToLowerInvariant();
        language.Locale = dto.Locale.Trim();
        language.Flag = dto.Flag;
        language.Direction = dto.Direction == "rtl" ? "rtl" : "ltr";
        language.SortOrder = dto.SortOrder;

        if (language.IsDefault && !dto.IsActive)
        {
            throw new ConflictException("Standart tilni faolsizlantirib bo'lmaydi.");
        }
        language.IsActive = dto.IsActive;

        language.UpdatedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync(ct);
        return ToDto(language);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var language = await FindAsync(id, ct);
        if (language.IsDefault)
        {
            throw new ConflictException("Standart tilni o'chirib bo'lmaydi.");
        }

        _context.Languages.Remove(language);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<LanguageDto> SetDefaultAsync(Guid id, CancellationToken ct = default)
    {
        var language = await FindAsync(id, ct);

        if (!language.IsActive)
        {
            throw new ConflictException("Nofaol tilni standart qilib bo'lmaydi.");
        }

        var currentDefault = await _context.Languages.Where(l => l.IsDefault && l.Id != id).ToListAsync(ct);
        foreach (var other in currentDefault)
        {
            other.IsDefault = false;
            other.UpdatedAt = DateTimeOffset.UtcNow;
        }

        language.IsDefault = true;
        language.UpdatedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync(ct);
        return ToDto(language);
    }

    public async Task<LanguageDto> ToggleStatusAsync(Guid id, CancellationToken ct = default)
    {
        var language = await FindAsync(id, ct);

        if (language.IsDefault && language.IsActive)
        {
            throw new ConflictException("Standart tilni faolsizlantirib bo'lmaydi.");
        }

        language.IsActive = !language.IsActive;
        language.UpdatedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync(ct);
        return ToDto(language);
    }

    public async Task ReorderAsync(List<LanguageReorderItemDto> items, CancellationToken ct = default)
    {
        var ids = items.Select(i => i.Id).ToList();
        var languages = await _context.Languages.Where(l => ids.Contains(l.Id)).ToListAsync(ct);

        foreach (var item in items)
        {
            var language = languages.FirstOrDefault(l => l.Id == item.Id);
            if (language is null) continue;
            language.SortOrder = item.SortOrder;
            language.UpdatedAt = DateTimeOffset.UtcNow;
        }

        await _context.SaveChangesAsync(ct);
    }

    private static void Validate(LanguageCreateDto dto)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(dto.Name)) errors["name"] = ["Nomi majburiy."];
        if (string.IsNullOrWhiteSpace(dto.NativeName)) errors["nativeName"] = ["Mahalliy nomi majburiy."];
        if (string.IsNullOrWhiteSpace(dto.Code)) errors["code"] = ["Kod majburiy."];
        if (string.IsNullOrWhiteSpace(dto.Locale)) errors["locale"] = ["Locale majburiy."];
        if (dto.Direction != "ltr" && dto.Direction != "rtl") errors["direction"] = ["Direction faqat 'ltr' yoki 'rtl' bo'lishi mumkin."];

        if (errors.Count > 0) throw new ValidationException(errors);
    }

    private async Task EnsureUniqueAsync(string code, string locale, Guid? excludeId, CancellationToken ct)
    {
        var normalizedCode = code.Trim().ToLowerInvariant();
        var codeExists = await _context.Languages.AnyAsync(l => l.Code == normalizedCode && l.Id != excludeId, ct);
        if (codeExists) throw new ConflictException($"\"{normalizedCode}\" kodi bilan til allaqachon mavjud.");

        var localeExists = await _context.Languages.AnyAsync(l => l.Locale == locale && l.Id != excludeId, ct);
        if (localeExists) throw new ConflictException($"\"{locale}\" locale bilan til allaqachon mavjud.");
    }

    private async Task<Language> FindAsync(Guid id, CancellationToken ct)
    {
        var language = await _context.Languages.FirstOrDefaultAsync(l => l.Id == id, ct);
        if (language is null) throw new NotFoundException(nameof(Language), id);
        return language;
    }

    private static LanguageDto ToDto(Language language) => new()
    {
        Id = language.Id,
        Name = language.Name,
        NativeName = language.NativeName,
        Code = language.Code,
        Locale = language.Locale,
        Flag = language.Flag,
        Direction = language.Direction,
        IsDefault = language.IsDefault,
        IsActive = language.IsActive,
        SortOrder = language.SortOrder,
        CreatedAt = language.CreatedAt,
        UpdatedAt = language.UpdatedAt,
    };
}
