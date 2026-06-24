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
            NameRu = dto.NameRu,
            Slug = slug,
            Description = dto.Description,
            ImageFileId = dto.ImageFileId,
            SortOrder = dto.SortOrder,
            IsActive = dto.IsActive,
            MetaTitleUz = dto.MetaTitleUz,
            MetaTitleRu = dto.MetaTitleRu,
            MetaDescriptionUz = dto.MetaDescriptionUz,
            MetaDescriptionRu = dto.MetaDescriptionRu,
            MetaKeywords = dto.MetaKeywords,
            IsIndexable = dto.IsIndexable,
            IsFollow = dto.IsFollow,
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
        category.NameRu = dto.NameRu;
        category.Slug = slug;
        category.Description = dto.Description;
        category.ImageFileId = dto.ImageFileId;
        category.SortOrder = dto.SortOrder;
        category.IsActive = dto.IsActive;
        category.MetaTitleUz = dto.MetaTitleUz;
        category.MetaTitleRu = dto.MetaTitleRu;
        category.MetaDescriptionUz = dto.MetaDescriptionUz;
        category.MetaDescriptionRu = dto.MetaDescriptionRu;
        category.MetaKeywords = dto.MetaKeywords;
        category.IsIndexable = dto.IsIndexable;
        category.IsFollow = dto.IsFollow;
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
        NameRu = category.NameRu,
        Slug = category.Slug,
        Description = category.Description,
        ImageUrl = category.ImageFile?.Url,
        SortOrder = category.SortOrder,
        IsActive = category.IsActive,
        ProductCount = productCount,
        MetaTitleUz = category.MetaTitleUz,
        MetaTitleRu = category.MetaTitleRu,
        MetaDescriptionUz = category.MetaDescriptionUz,
        MetaDescriptionRu = category.MetaDescriptionRu,
        MetaKeywords = category.MetaKeywords,
        IsIndexable = category.IsIndexable,
        IsFollow = category.IsFollow,
    };
}
