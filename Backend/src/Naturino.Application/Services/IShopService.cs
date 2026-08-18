using Naturino.Application.Common;
using Naturino.Application.DTOs.Shops;

namespace Naturino.Application.Services;

public interface IShopService
{
    Task<PagedResult<ShopDto>> GetPagedAsync(ShopQueryDto query, CancellationToken ct = default);
    Task<List<ShopDto>> GetAllActiveAsync(CancellationToken ct = default);
    Task<ShopDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ShopDto> CreateAsync(ShopCreateDto dto, CancellationToken ct = default);
    Task<ShopDto> UpdateAsync(Guid id, ShopUpdateDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<ShopDto> ToggleStatusAsync(Guid id, CancellationToken ct = default);
}
