using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Naturino.Application.DTOs.Products;
using Naturino.Application.Services;
using Naturino.Domain.Entities;
using Naturino.Domain.Exceptions;
using Naturino.Infrastructure.Common;
using Naturino.Infrastructure.Persistence;

namespace Naturino.Infrastructure.Repositories;

public class ProductCategoryService : IProductCategoryService
{
    private readonly ApplicationDbContext _context;

    public ProductCategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductCategoryDto>> GetAllAsync(CancellationToken ct = default)
    {
        var categories = await _context.ProductCategories
            .Include(c => c.ImageFile)
            .OrderBy(c => c.SortOrder)
            .ToListAsync(ct);

        var counts = await _context.Products
            .Where(p => !p.IsDeleted)
            .GroupBy(p => p.CategoryId)
            .Select(g => new { CategoryId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.CategoryId, g => g.Count, ct);

        return categories.Select((c) => ToDto(c, counts.GetValueOrDefault(c.Id))).ToList();
    }

    public async Task<ProductCategoryDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var category = await FindAsync(id, ct);
        var count = await _context.Products.CountAsync(p => p.CategoryId == id && !p.IsDeleted, ct);
        return ToDto(category, count);
    }

    public async Task<ProductCategoryDto> CreateAsync(ProductCategoryCreateDto dto, CancellationToken ct = default)
    {
        var slug = string.IsNullOrWhiteSpace(dto.Slug) ? SlugHelper.Generate(dto.Name) : SlugHelper.Generate(dto.Slug);
        await EnsureUniqueSlugAsync(slug, null, ct);

        var category = new ProductCategory
        {
            ParentCategoryId = dto.ParentCategoryId,
            Name = dto.Name,
            Slug = slug,
            Description = dto.Description,
            ImageFileId = dto.ImageFileId,
            SortOrder = dto.SortOrder,
            IsActive = dto.IsActive,
            MetaTitleUz = dto.MetaTitleUz,
            MetaDescriptionUz = dto.MetaDescriptionUz,
            MetaKeywords = dto.MetaKeywords,
            IsIndexable = dto.IsIndexable,
            IsFollow = dto.IsFollow,
            Translations = SerializeTranslations(dto.Translations),
        };

        _context.ProductCategories.Add(category);
        await _context.SaveChangesAsync(ct);

        return await GetByIdAsync(category.Id, ct);
    }

    public async Task<ProductCategoryDto> UpdateAsync(Guid id, ProductCategoryUpdateDto dto, CancellationToken ct = default)
    {
        var category = await FindAsync(id, ct);

        var slug = string.IsNullOrWhiteSpace(dto.Slug) ? SlugHelper.Generate(dto.Name) : SlugHelper.Generate(dto.Slug);
        await EnsureUniqueSlugAsync(slug, id, ct);

        category.ParentCategoryId = dto.ParentCategoryId;
        category.Name = dto.Name;
        category.Slug = slug;
        category.Description = dto.Description;
        category.ImageFileId = dto.ImageFileId;
        category.SortOrder = dto.SortOrder;
        category.IsActive = dto.IsActive;
        category.MetaTitleUz = dto.MetaTitleUz;
        category.MetaDescriptionUz = dto.MetaDescriptionUz;
        category.MetaKeywords = dto.MetaKeywords;
        category.IsIndexable = dto.IsIndexable;
        category.IsFollow = dto.IsFollow;
        category.Translations = SerializeTranslations(dto.Translations);
        category.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(ct);

        return await GetByIdAsync(category.Id, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var category = await FindAsync(id, ct);

        var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id && !p.IsDeleted, ct);
        if (hasProducts)
        {
            throw new ConflictException("Bu kategoriyada mahsulotlar mavjud, avval ularni o'chiring yoki ko'chiring.");
        }

        _context.ProductCategories.Remove(category);
        await _context.SaveChangesAsync(ct);
    }

    public async Task ReorderAsync(List<ProductCategoryReorderItemDto> items, CancellationToken ct = default)
    {
        var ids = items.Select(i => i.Id).ToList();
        var categories = await _context.ProductCategories.Where(c => ids.Contains(c.Id)).ToListAsync(ct);

        foreach (var item in items)
        {
            var category = categories.FirstOrDefault(c => c.Id == item.Id);
            if (category is null) continue;
            category.SortOrder = item.SortOrder;
            category.UpdatedAt = DateTimeOffset.UtcNow;
        }

        await _context.SaveChangesAsync(ct);
    }

    private async Task<ProductCategory> FindAsync(Guid id, CancellationToken ct)
    {
        return await _context.ProductCategories
            .Include(c => c.ImageFile)
            .FirstOrDefaultAsync(c => c.Id == id, ct)
            ?? throw new NotFoundException(nameof(ProductCategory), id);
    }

    private async Task EnsureUniqueSlugAsync(string slug, Guid? excludeId, CancellationToken ct)
    {
        var exists = await _context.ProductCategories.AnyAsync(c => c.Slug == slug && c.Id != excludeId, ct);
        if (exists)
        {
            throw new ConflictException($"Slug \"{slug}\" allaqachon mavjud.");
        }
    }

    private static ProductCategoryDto ToDto(ProductCategory category, int productCount) => new()
    {
        Id = category.Id,
        ParentCategoryId = category.ParentCategoryId,
        Name = category.Name,
        Slug = category.Slug,
        Description = category.Description,
        ImageUrl = category.ImageFile?.Url,
        SortOrder = category.SortOrder,
        IsActive = category.IsActive,
        ProductCount = productCount,
        MetaTitleUz = category.MetaTitleUz,
        MetaDescriptionUz = category.MetaDescriptionUz,
        MetaKeywords = category.MetaKeywords,
        IsIndexable = category.IsIndexable,
        IsFollow = category.IsFollow,
        Translations = DeserializeTranslations(category),
    };

    private static string? SerializeTranslations(Dictionary<string, CategoryTranslationDto>? translations)
    {
        if (translations is null || translations.Count == 0) return null;
        return JsonSerializer.Serialize(translations);
    }

    /// <summary>Reads the Translations jsonb column, then fills in a "ru" entry from the legacy
    /// NameRu/MetaTitleRu/MetaDescriptionRu columns when Translations has none — so data entered
    /// before this column existed keeps showing up after the admin form switches to translations-only editing.</summary>
    private static Dictionary<string, CategoryTranslationDto> DeserializeTranslations(ProductCategory category)
    {
        var result = string.IsNullOrWhiteSpace(category.Translations)
            ? new Dictionary<string, CategoryTranslationDto>()
            : JsonSerializer.Deserialize<Dictionary<string, CategoryTranslationDto>>(category.Translations) ?? [];

        var hasLegacyRu = category.NameRu is not null || category.MetaTitleRu is not null || category.MetaDescriptionRu is not null;
        if (hasLegacyRu && !result.ContainsKey("ru"))
        {
            result["ru"] = new CategoryTranslationDto
            {
                Name = category.NameRu,
                MetaTitle = category.MetaTitleRu,
                MetaDescription = category.MetaDescriptionRu,
            };
        }

        return result;
    }
}
