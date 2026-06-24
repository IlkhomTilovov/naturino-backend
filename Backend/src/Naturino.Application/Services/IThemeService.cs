using Naturino.Application.DTOs.Themes;

namespace Naturino.Application.Services;

public interface IThemeService
{
    Task<List<ThemeDto>> GetAllAsync(CancellationToken ct = default);
    Task<ThemeDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ThemeDto> GetActiveAsync(CancellationToken ct = default);
    Task<ThemeDto> CreateAsync(ThemeCreateDto dto, CancellationToken ct = default);
    Task<ThemeDto> UpdateAsync(Guid id, ThemeUpdateDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<ThemeDto> DuplicateAsync(Guid id, CancellationToken ct = default);
    Task<ThemeDto> SetActiveAsync(Guid id, CancellationToken ct = default);
}
