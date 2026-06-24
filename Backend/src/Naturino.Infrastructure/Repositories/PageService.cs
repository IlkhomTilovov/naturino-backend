using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Naturino.Application.DTOs.Pages;
using Naturino.Application.Services;
using Naturino.Domain.Entities;
using Naturino.Domain.Exceptions;
using Naturino.Infrastructure.Common;
using Naturino.Infrastructure.Persistence;

namespace Naturino.Infrastructure.Repositories;

public class PageService : IPageService
{
    private readonly ApplicationDbContext _context;

    public PageService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PageDto>> GetAllAsync(CancellationToken ct = default)
    {
        var pages = await _context.Pages
            .Include(p => p.Sections)
            .Include(p => p.OgImageFile)
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.Title)
            .ToListAsync(ct);

        return pages.Select(p => ToDto(p, preferDraft: false)).ToList();
    }

    public async Task<PageDto> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        var page = await _context.Pages
            .Include(p => p.Sections)
            .Include(p => p.OgImageFile)
            .FirstOrDefaultAsync(p => p.Slug == slug && !p.IsDeleted, ct)
            ?? throw new NotFoundException(nameof(Page), slug);

        return ToDto(page, preferDraft: false);
    }

    public async Task<PageDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var page = await FindAsync(id, ct);
        return ToDto(page, preferDraft: true);
    }

    public async Task<PageDto> CreateAsync(CreatePageDto dto, Guid? userId, CancellationToken ct = default)
    {
        var slug = SlugHelper.Generate(dto.Slug);

        var exists = await _context.Pages.AnyAsync(p => p.Slug == slug && !p.IsDeleted, ct);
        if (exists)
        {
            throw new ConflictException($"Slug \"{slug}\" allaqachon mavjud.");
        }

        var page = new Page
        {
            Slug = slug,
            Title = dto.Title,
            IsHomePage = false,
            IsPublished = dto.IsPublished,
            MetaTitle = dto.MetaTitle,
            MetaDescription = dto.MetaDescription,
            IsIndexable = dto.IsIndexable,
            IsFollow = dto.IsFollow,
            CreatedBy = userId,
            UpdatedBy = userId,
        };

        _context.Pages.Add(page);
        await _context.SaveChangesAsync(ct);

        return await GetByIdAsync(page.Id, ct);
    }

    public async Task<PageDto> UpdateAsync(Guid id, UpdatePageDto dto, Guid? userId, CancellationToken ct = default)
    {
        var page = await FindAsync(id, ct);

        page.Title = dto.Title;
        page.IsPublished = dto.IsPublished;
        page.MetaTitle = dto.MetaTitle;
        page.MetaDescription = dto.MetaDescription;
        page.MetaKeywords = dto.MetaKeywords;
        page.OgImageFileId = dto.OgImageFileId;
        page.IsIndexable = dto.IsIndexable;
        page.IsFollow = dto.IsFollow;
        page.UpdatedBy = userId;
        page.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(ct);

        return await GetByIdAsync(page.Id, ct);
    }

    public async Task<PageSectionDto> UpdateSectionAsync(Guid sectionId, UpdatePageSectionDto dto, CancellationToken ct = default)
    {
        var section = await _context.PageSections.FirstOrDefaultAsync(s => s.Id == sectionId, ct)
            ?? throw new NotFoundException(nameof(PageSection), sectionId);

        section.SortOrder = dto.SortOrder;
        section.IsEnabled = dto.IsEnabled;
        section.DraftContent = dto.Content.ValueKind == JsonValueKind.Undefined ? "{}" : dto.Content.GetRawText();
        section.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(ct);

        return ToSectionDto(section, preferDraft: true);
    }

    public async Task<PageDto> PublishAsync(Guid pageId, Guid? userId, CancellationToken ct = default)
    {
        var page = await FindAsync(pageId, ct);

        foreach (var section in page.Sections)
        {
            if (section.DraftContent is not null)
            {
                section.Content = section.DraftContent;
                section.DraftContent = null;
                section.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }

        page.IsPublished = true;
        page.UpdatedBy = userId;
        page.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(ct);

        return ToDto(page, preferDraft: true);
    }

    public async Task<PageSectionDto> AddSectionAsync(Guid pageId, CreatePageSectionDto dto, CancellationToken ct = default)
    {
        var page = await FindAsync(pageId, ct);

        var section = new PageSection
        {
            PageId = page.Id,
            SectionType = dto.SectionType,
            SortOrder = dto.SortOrder,
            IsEnabled = true,
            Content = dto.Content.ValueKind == JsonValueKind.Undefined ? "{}" : dto.Content.GetRawText(),
        };

        _context.PageSections.Add(section);
        await _context.SaveChangesAsync(ct);

        return ToSectionDto(section, preferDraft: true);
    }

    public async Task DeleteSectionAsync(Guid sectionId, CancellationToken ct = default)
    {
        var section = await _context.PageSections.FirstOrDefaultAsync(s => s.Id == sectionId, ct)
            ?? throw new NotFoundException(nameof(PageSection), sectionId);

        _context.PageSections.Remove(section);
        await _context.SaveChangesAsync(ct);
    }

    private async Task<Page> FindAsync(Guid id, CancellationToken ct)
    {
        return await _context.Pages
            .Include(p => p.Sections)
            .Include(p => p.OgImageFile)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, ct)
            ?? throw new NotFoundException(nameof(Page), id);
    }

    private static PageDto ToDto(Page page, bool preferDraft) => new()
    {
        Id = page.Id,
        Slug = page.Slug,
        Title = page.Title,
        IsHomePage = page.IsHomePage,
        IsPublished = page.IsPublished,
        UpdatedAt = page.UpdatedAt,
        MetaTitle = page.MetaTitle,
        MetaDescription = page.MetaDescription,
        MetaKeywords = page.MetaKeywords,
        OgImageUrl = page.OgImageFile?.Url,
        IsIndexable = page.IsIndexable,
        IsFollow = page.IsFollow,
        Sections = page.Sections.OrderBy(s => s.SortOrder).Select(s => ToSectionDto(s, preferDraft)).ToList()
    };

    private static PageSectionDto ToSectionDto(PageSection section, bool preferDraft) => new()
    {
        Id = section.Id,
        PageId = section.PageId,
        SectionType = section.SectionType,
        SortOrder = section.SortOrder,
        IsEnabled = section.IsEnabled,
        Content = DeserializeContent(preferDraft ? section.DraftContent ?? section.Content : section.Content),
        HasUnpublishedChanges = section.DraftContent is not null,
    };

    private static JsonElement DeserializeContent(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<JsonElement>(json);
        }
        catch (JsonException)
        {
            return JsonSerializer.Deserialize<JsonElement>("{}");
        }
    }
}
