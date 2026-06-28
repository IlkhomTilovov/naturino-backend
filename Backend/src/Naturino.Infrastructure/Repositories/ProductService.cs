using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Naturino.Application.Common;
using Naturino.Application.DTOs.Products;
using Naturino.Application.Services;
using Naturino.Domain.Entities;
using Naturino.Domain.Exceptions;
using Naturino.Infrastructure.Common;
using Naturino.Infrastructure.Persistence;

namespace Naturino.Infrastructure.Repositories;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ProductDto>> GetPagedAsync(ProductQueryDto query, CancellationToken ct = default)
    {
        var products = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images).ThenInclude(i => i.MediaFile)
            .Where(p => !p.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            products = products.Where(p => p.Name.ToLower().Contains(term) || p.SKU.ToLower().Contains(term));
        }

        if (query.CategoryId is not null)
        {
            products = products.Where(p => p.CategoryId == query.CategoryId);
        }

        if (query.IsActive is not null)
        {
            products = products.Where(p => p.IsActive == query.IsActive);
        }

        if (query.IsFeatured is not null)
        {
            products = products.Where(p => p.IsFeatured == query.IsFeatured);
        }

        var totalCount = await products.CountAsync(ct);

        var items = await products
            .OrderByDescending(p => p.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        return PagedResult<ProductDto>.Create(items.Select(ToDto).ToList(), query.Page, query.PageSize, totalCount);
    }

    public async Task<ProductDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var product = await FindAsync(id, ct);
        return ToDto(product);
    }

    public async Task<ProductDto> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images).ThenInclude(i => i.MediaFile)
            .FirstOrDefaultAsync(p => p.Slug == slug && !p.IsDeleted, ct)
            ?? throw new NotFoundException(nameof(Product), slug);

        return ToDto(product);
    }

    public async Task<List<ProductDto>> GetRelatedAsync(Guid productId, int take, CancellationToken ct = default)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId, ct);
        if (product is null) return [];

        var related = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images).ThenInclude(i => i.MediaFile)
            .Where(p => p.CategoryId == product.CategoryId && p.Id != productId && p.IsActive && !p.IsDeleted)
            .OrderByDescending(p => p.IsFeatured)
            .ThenByDescending(p => p.CreatedAt)
            .Take(take)
            .ToListAsync(ct);

        return related.Select(ToDto).ToList();
    }

    public async Task<ProductDto> CreateAsync(ProductCreateDto dto, Guid? userId, CancellationToken ct = default)
    {
        await EnsureCategoryExistsAsync(dto.CategoryId, ct);

        var slug = string.IsNullOrWhiteSpace(dto.Slug) ? SlugHelper.Generate(dto.Name) : SlugHelper.Generate(dto.Slug);
        await EnsureUniqueSlugAsync(slug, null, ct);
        await EnsureUniqueSkuAsync(dto.SKU, null, ct);

        var product = new Product
        {
            CategoryId = dto.CategoryId,
            SKU = dto.SKU,
            Name = dto.Name,
            Slug = slug,
            ShortDescription = dto.ShortDescription,
            Description = dto.Description,
            Price = dto.Price,
            OldPrice = dto.OldPrice,
            StockQuantity = dto.StockQuantity,
            Weight = dto.Weight,
            Brand = dto.Brand,
            AgeGroup = dto.AgeGroup,
            IsFeatured = dto.IsFeatured,
            IsActive = dto.IsActive,
            Translations = SerializeTranslations(dto.Translations),
            NutritionalInfo = SerializeList(dto.NutritionalInfo),
            PackagingOptions = SerializeList(dto.PackagingOptions),
            IngredientsList = SerializeList(dto.IngredientsList),
            Certifications = SerializeList(dto.Certifications),
            ExportInfo = dto.ExportInfo is null ? null : JsonSerializer.Serialize(dto.ExportInfo),
            CreatedBy = userId,
            UpdatedBy = userId
        };

        ApplyImages(product, dto.MediaFileIds);

        _context.Products.Add(product);
        await _context.SaveChangesAsync(ct);

        return await GetByIdAsync(product.Id, ct);
    }

    public async Task<ProductDto> UpdateAsync(Guid id, ProductUpdateDto dto, Guid? userId, CancellationToken ct = default)
    {
        var product = await FindAsync(id, ct);

        await EnsureCategoryExistsAsync(dto.CategoryId, ct);

        var slug = string.IsNullOrWhiteSpace(dto.Slug) ? SlugHelper.Generate(dto.Name) : SlugHelper.Generate(dto.Slug);
        await EnsureUniqueSlugAsync(slug, id, ct);
        await EnsureUniqueSkuAsync(dto.SKU, id, ct);

        product.CategoryId = dto.CategoryId;
        product.SKU = dto.SKU;
        product.Name = dto.Name;
        product.Slug = slug;
        product.ShortDescription = dto.ShortDescription;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.OldPrice = dto.OldPrice;
        product.StockQuantity = dto.StockQuantity;
        product.Weight = dto.Weight;
        product.Brand = dto.Brand;
        product.AgeGroup = dto.AgeGroup;
        product.IsFeatured = dto.IsFeatured;
        product.IsActive = dto.IsActive;
        product.Translations = SerializeTranslations(dto.Translations);
        product.NutritionalInfo = SerializeList(dto.NutritionalInfo);
        product.PackagingOptions = SerializeList(dto.PackagingOptions);
        product.IngredientsList = SerializeList(dto.IngredientsList);
        product.Certifications = SerializeList(dto.Certifications);
        product.ExportInfo = dto.ExportInfo is null ? null : JsonSerializer.Serialize(dto.ExportInfo);
        product.UpdatedBy = userId;
        product.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(ct);

        return await GetByIdAsync(product.Id, ct);
    }

    public async Task<List<ProductImageDto>> GetImagesAsync(Guid productId, CancellationToken ct = default)
    {
        var product = await FindAsync(productId, ct);
        return product.Images.OrderBy(i => i.SortOrder).Select(MapImage).ToList();
    }

    public async Task<List<ProductImageDto>> AddImagesAsync(Guid productId, List<Guid> mediaFileIds, CancellationToken ct = default)
    {
        var product = await FindAsync(productId, ct);

        var hadNoImages = product.Images.Count == 0;
        var nextSortOrder = product.Images.Count == 0 ? 0 : product.Images.Max(i => i.SortOrder) + 1;

        for (var i = 0; i < mediaFileIds.Count; i++)
        {
            product.Images.Add(new ProductImage
            {
                MediaFileId = mediaFileIds[i],
                IsPrimary = hadNoImages && i == 0,
                SortOrder = nextSortOrder + i
            });
        }

        await _context.SaveChangesAsync(ct);

        var saved = await FindAsync(productId, ct);
        return saved.Images.OrderBy(i => i.SortOrder).Select(MapImage).ToList();
    }

    public async Task RemoveImageAsync(Guid productId, Guid imageId, CancellationToken ct = default)
    {
        var product = await FindAsync(productId, ct);
        var image = product.Images.FirstOrDefault(i => i.Id == imageId)
            ?? throw new NotFoundException(nameof(ProductImage), imageId);

        var wasPrimary = image.IsPrimary;
        product.Images.Remove(image);

        if (wasPrimary)
        {
            var next = product.Images.OrderBy(i => i.SortOrder).FirstOrDefault();
            if (next is not null) next.IsPrimary = true;
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task SetCoverImageAsync(Guid productId, Guid imageId, CancellationToken ct = default)
    {
        var product = await FindAsync(productId, ct);
        var target = product.Images.FirstOrDefault(i => i.Id == imageId)
            ?? throw new NotFoundException(nameof(ProductImage), imageId);

        foreach (var image in product.Images)
        {
            image.IsPrimary = image.Id == target.Id;
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task ReorderImagesAsync(Guid productId, List<Guid> orderedImageIds, CancellationToken ct = default)
    {
        var product = await FindAsync(productId, ct);

        var currentIds = product.Images.Select(i => i.Id).ToHashSet();
        if (currentIds.Count != orderedImageIds.Count || !currentIds.SetEquals(orderedImageIds))
        {
            throw new Domain.Exceptions.ValidationException(new Dictionary<string, string[]>
            {
                ["imageIds"] = ["Rasm ro'yxati mahsulotning mavjud rasmlariga to'liq mos kelishi kerak."]
            });
        }

        for (var i = 0; i < orderedImageIds.Count; i++)
        {
            var image = product.Images.First(img => img.Id == orderedImageIds[i]);
            image.SortOrder = i;
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task<ProductImageDto> UpdateImageAsync(Guid productId, Guid imageId, string? altText, CancellationToken ct = default)
    {
        var product = await FindAsync(productId, ct);
        var image = product.Images.FirstOrDefault(i => i.Id == imageId)
            ?? throw new NotFoundException(nameof(ProductImage), imageId);

        image.MediaFile.AltText = altText;
        await _context.SaveChangesAsync(ct);

        return MapImage(image);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var product = await FindAsync(id, ct);
        product.IsDeleted = true;
        product.DeletedAt = DateTimeOffset.UtcNow;
        product.IsActive = false;
        await _context.SaveChangesAsync(ct);
    }

    public async Task SetActiveAsync(Guid id, bool isActive, CancellationToken ct = default)
    {
        var product = await FindAsync(id, ct);
        product.IsActive = isActive;
        await _context.SaveChangesAsync(ct);
    }

    private async Task<Product> FindAsync(Guid id, CancellationToken ct)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images).ThenInclude(i => i.MediaFile)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, ct)
            ?? throw new NotFoundException(nameof(Product), id);
    }

    private async Task EnsureCategoryExistsAsync(Guid categoryId, CancellationToken ct)
    {
        var exists = await _context.ProductCategories.AnyAsync(c => c.Id == categoryId, ct);
        if (!exists)
        {
            throw new Domain.Exceptions.ValidationException(new Dictionary<string, string[]>
            {
                ["categoryId"] = ["Tanlangan kategoriya topilmadi."]
            });
        }
    }

    private async Task EnsureUniqueSlugAsync(string slug, Guid? excludeId, CancellationToken ct)
    {
        var exists = await _context.Products.AnyAsync(p => p.Slug == slug && p.Id != excludeId, ct);
        if (exists)
        {
            throw new ConflictException($"Slug \"{slug}\" allaqachon mavjud.");
        }
    }

    private async Task EnsureUniqueSkuAsync(string sku, Guid? excludeId, CancellationToken ct)
    {
        var exists = await _context.Products.AnyAsync(p => p.SKU == sku && p.Id != excludeId, ct);
        if (exists)
        {
            throw new ConflictException($"SKU \"{sku}\" allaqachon mavjud.");
        }
    }

    private static void ApplyImages(Product product, List<Guid> mediaFileIds)
    {
        for (var i = 0; i < mediaFileIds.Count; i++)
        {
            product.Images.Add(new ProductImage
            {
                MediaFileId = mediaFileIds[i],
                IsPrimary = i == 0,
                SortOrder = i
            });
        }
    }

    private static ProductDto ToDto(Product product) => new()
    {
        Id = product.Id,
        CategoryId = product.CategoryId,
        CategoryName = product.Category?.Name ?? string.Empty,
        SKU = product.SKU,
        Name = product.Name,
        Slug = product.Slug,
        ShortDescription = product.ShortDescription,
        Description = product.Description,
        Price = product.Price,
        OldPrice = product.OldPrice,
        StockQuantity = product.StockQuantity,
        Weight = product.Weight,
        Brand = product.Brand,
        AgeGroup = product.AgeGroup,
        IsFeatured = product.IsFeatured,
        IsActive = product.IsActive,
        CreatedAt = product.CreatedAt,
        UpdatedAt = product.UpdatedAt,
        Images = product.Images.OrderBy(i => i.SortOrder).Select(MapImage).ToList(),
        Translations = DeserializeTranslations(product.Translations),
        NutritionalInfo = DeserializeList<NutritionalItemDto>(product.NutritionalInfo),
        PackagingOptions = DeserializeList<PackagingOptionDto>(product.PackagingOptions),
        IngredientsList = DeserializeList<IngredientItemDto>(product.IngredientsList),
        Certifications = DeserializeList<ProductCertificationDto>(product.Certifications),
        ExportInfo = string.IsNullOrWhiteSpace(product.ExportInfo)
            ? new ExportInfoDto()
            : JsonSerializer.Deserialize<ExportInfoDto>(product.ExportInfo) ?? new ExportInfoDto(),
    };

    private static string? SerializeTranslations(Dictionary<string, ProductTranslationDto>? translations)
    {
        if (translations is null || translations.Count == 0) return null;
        return JsonSerializer.Serialize(translations);
    }

    private static string? SerializeList<T>(List<T>? items)
    {
        if (items is null || items.Count == 0) return null;
        return JsonSerializer.Serialize(items);
    }

    private static List<T> DeserializeList<T>(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return [];
        return JsonSerializer.Deserialize<List<T>>(json) ?? [];
    }

    private static Dictionary<string, ProductTranslationDto> DeserializeTranslations(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return [];
        return JsonSerializer.Deserialize<Dictionary<string, ProductTranslationDto>>(json) ?? [];
    }

    private static ProductImageDto MapImage(ProductImage i) => new()
    {
        Id = i.Id,
        MediaFileId = i.MediaFileId,
        Url = i.MediaFile.Url,
        AltText = i.MediaFile.AltText,
        IsPrimary = i.IsPrimary,
        SortOrder = i.SortOrder
    };
}
