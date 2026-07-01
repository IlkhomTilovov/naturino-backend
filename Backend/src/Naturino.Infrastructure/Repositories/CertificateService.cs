using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Naturino.Application.DTOs.Certificates;
using Naturino.Application.Services;
using Naturino.Domain.Entities;
using Naturino.Domain.Exceptions;
using Naturino.Infrastructure.Persistence;

namespace Naturino.Infrastructure.Repositories;

public class CertificateService : ICertificateService
{
    private readonly ApplicationDbContext _context;
    private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

    public CertificateService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CertificateDto>> GetAllAsync(CancellationToken ct = default)
    {
        var certs = await _context.Certificates
            .Where(c => !c.IsDeleted)
            .Include(c => c.File)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.CreatedAt)
            .ToListAsync(ct);
        return certs.Select(ToDto).ToList();
    }

    public async Task<CertificateDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var cert = await FindAsync(id, ct);
        return ToDto(cert);
    }

    public async Task<CertificateDto> CreateAsync(CertificateCreateDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new ValidationException(new Dictionary<string, string[]> { ["title"] = ["Sarlavha majburiy."] });

        var cert = new Certificate
        {
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            IssuedBy = dto.IssuedBy?.Trim(),
            CertificateNumber = dto.CertificateNumber?.Trim(),
            IssueDate = dto.IssueDate,
            ExpiryDate = dto.ExpiryDate,
            Icon = dto.Icon,
            Category = dto.Category,
            Scope = dto.Scope,
            VerificationUrl = dto.VerificationUrl?.Trim(),
            TranslationsJson = SerializeTranslations(dto.Translations),
            FileId = dto.FileId,
            SortOrder = dto.SortOrder,
            IsActive = dto.IsActive,
        };

        _context.Certificates.Add(cert);
        await _context.SaveChangesAsync(ct);
        await _context.Entry(cert).Reference(c => c.File).LoadAsync(ct);
        return ToDto(cert);
    }

    public async Task<CertificateDto> UpdateAsync(Guid id, CertificateUpdateDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new ValidationException(new Dictionary<string, string[]> { ["title"] = ["Sarlavha majburiy."] });

        var cert = await FindAsync(id, ct);

        cert.Title = dto.Title.Trim();
        cert.Description = dto.Description?.Trim();
        cert.IssuedBy = dto.IssuedBy?.Trim();
        cert.CertificateNumber = dto.CertificateNumber?.Trim();
        cert.IssueDate = dto.IssueDate;
        cert.ExpiryDate = dto.ExpiryDate;
        cert.Icon = dto.Icon;
        cert.Category = dto.Category;
        cert.Scope = dto.Scope;
        cert.VerificationUrl = dto.VerificationUrl?.Trim();
        cert.TranslationsJson = SerializeTranslations(dto.Translations);
        cert.FileId = dto.FileId;
        cert.SortOrder = dto.SortOrder;
        cert.IsActive = dto.IsActive;
        cert.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(ct);
        await _context.Entry(cert).Reference(c => c.File).LoadAsync(ct);
        return ToDto(cert);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var cert = await FindAsync(id, ct);
        cert.IsDeleted = true;
        cert.DeletedAt = DateTimeOffset.UtcNow;
        cert.UpdatedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync(ct);
    }

    public async Task<CertificateDto> ToggleStatusAsync(Guid id, CancellationToken ct = default)
    {
        var cert = await FindAsync(id, ct);
        cert.IsActive = !cert.IsActive;
        cert.UpdatedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync(ct);
        return ToDto(cert);
    }

    public async Task ReorderAsync(List<CertificateReorderItemDto> items, CancellationToken ct = default)
    {
        var ids = items.Select(i => i.Id).ToList();
        var certs = await _context.Certificates.Where(c => ids.Contains(c.Id)).ToListAsync(ct);
        foreach (var item in items)
        {
            var cert = certs.FirstOrDefault(c => c.Id == item.Id);
            if (cert is null) continue;
            cert.SortOrder = item.SortOrder;
            cert.UpdatedAt = DateTimeOffset.UtcNow;
        }
        await _context.SaveChangesAsync(ct);
    }

    private async Task<Certificate> FindAsync(Guid id, CancellationToken ct)
    {
        var cert = await _context.Certificates
            .Include(c => c.File)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, ct);
        if (cert is null) throw new NotFoundException(nameof(Certificate), id);
        return cert;
    }

    private static string SerializeTranslations(Dictionary<string, CertificateTranslationDto> translations)
    {
        if (translations is null || translations.Count == 0) return "{}";
        return JsonSerializer.Serialize(translations);
    }

    private static Dictionary<string, CertificateTranslationDto> DeserializeTranslations(string? json)
    {
        if (string.IsNullOrWhiteSpace(json) || json == "{}") return [];
        try { return JsonSerializer.Deserialize<Dictionary<string, CertificateTranslationDto>>(json, _json) ?? []; }
        catch { return []; }
    }

    private static CertificateDto ToDto(Certificate c) => new()
    {
        Id = c.Id,
        Title = c.Title,
        Description = c.Description,
        IssuedBy = c.IssuedBy,
        CertificateNumber = c.CertificateNumber,
        IssueDate = c.IssueDate,
        ExpiryDate = c.ExpiryDate,
        Icon = c.Icon,
        Category = c.Category,
        Scope = c.Scope,
        VerificationUrl = c.VerificationUrl,
        Translations = DeserializeTranslations(c.TranslationsJson),
        FileId = c.FileId,
        FileUrl = c.File?.Url,
        SortOrder = c.SortOrder,
        IsActive = c.IsActive,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt,
    };
}
