using Microsoft.EntityFrameworkCore;
using Naturino.Application.Common;
using Naturino.Application.DTOs.Shops;
using Naturino.Application.Services;
using Naturino.Domain.Entities;
using Naturino.Domain.Exceptions;
using Naturino.Infrastructure.Persistence;

namespace Naturino.Infrastructure.Repositories;

public class ShopService : IShopService
{
    private readonly ApplicationDbContext _context;

    public ShopService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ShopDto>> GetPagedAsync(ShopQueryDto query, CancellationToken ct = default)
    {
        var shops = _context.Shops.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            shops = shops.Where(s =>
                s.Name.ToLower().Contains(term) ||
                s.City.ToLower().Contains(term) ||
                s.Country.ToLower().Contains(term) ||
                s.Address.ToLower().Contains(term));
        }

        if (query.IsActive is not null)
        {
            shops = shops.Where(s => s.IsActive == query.IsActive);
        }

        var totalCount = await shops.CountAsync(ct);

        var items = await shops
            .OrderBy(s => s.Country).ThenBy(s => s.City).ThenBy(s => s.Name)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        return PagedResult<ShopDto>.Create(items.Select(ToDto).ToList(), query.Page, query.PageSize, totalCount);
    }

    public async Task<List<ShopDto>> GetAllActiveAsync(CancellationToken ct = default)
    {
        var shops = await _context.Shops
            .Where(s => s.IsActive)
            .OrderBy(s => s.Country).ThenBy(s => s.City).ThenBy(s => s.Name)
            .ToListAsync(ct);

        return shops.Select(ToDto).ToList();
    }

    public async Task<ShopDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var shop = await FindAsync(id, ct);
        return ToDto(shop);
    }

    public async Task<ShopDto> CreateAsync(ShopCreateDto dto, CancellationToken ct = default)
    {
        Validate(dto);

        var shop = new Shop
        {
            Name = dto.Name.Trim(),
            Country = dto.Country.Trim(),
            City = dto.City.Trim(),
            Address = dto.Address.Trim(),
            Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim(),
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            IsActive = dto.IsActive,
        };

        _context.Shops.Add(shop);
        await _context.SaveChangesAsync(ct);
        return ToDto(shop);
    }

    public async Task<ShopDto> UpdateAsync(Guid id, ShopUpdateDto dto, CancellationToken ct = default)
    {
        Validate(dto);
        var shop = await FindAsync(id, ct);

        shop.Name = dto.Name.Trim();
        shop.Country = dto.Country.Trim();
        shop.City = dto.City.Trim();
        shop.Address = dto.Address.Trim();
        shop.Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim();
        shop.Latitude = dto.Latitude;
        shop.Longitude = dto.Longitude;
        shop.IsActive = dto.IsActive;
        shop.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(ct);
        return ToDto(shop);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var shop = await FindAsync(id, ct);
        _context.Shops.Remove(shop);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<ShopDto> ToggleStatusAsync(Guid id, CancellationToken ct = default)
    {
        var shop = await FindAsync(id, ct);
        shop.IsActive = !shop.IsActive;
        shop.UpdatedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync(ct);
        return ToDto(shop);
    }

    private static void Validate(ShopCreateDto dto)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(dto.Name)) errors["name"] = ["Nomi majburiy."];
        if (string.IsNullOrWhiteSpace(dto.Country)) errors["country"] = ["Davlat majburiy."];
        if (string.IsNullOrWhiteSpace(dto.City)) errors["city"] = ["Shahar majburiy."];
        if (string.IsNullOrWhiteSpace(dto.Address)) errors["address"] = ["Manzil majburiy."];

        if (errors.Count > 0) throw new ValidationException(errors);
    }

    private async Task<Shop> FindAsync(Guid id, CancellationToken ct)
    {
        var shop = await _context.Shops.FirstOrDefaultAsync(s => s.Id == id, ct);
        if (shop is null) throw new NotFoundException(nameof(Shop), id);
        return shop;
    }

    private static ShopDto ToDto(Shop shop) => new()
    {
        Id = shop.Id,
        Name = shop.Name,
        Country = shop.Country,
        City = shop.City,
        Address = shop.Address,
        Phone = shop.Phone,
        Latitude = shop.Latitude,
        Longitude = shop.Longitude,
        IsActive = shop.IsActive,
        CreatedAt = shop.CreatedAt,
        UpdatedAt = shop.UpdatedAt,
    };
}
