using Naturino.Application.DTOs.Languages;

namespace Naturino.Application.Services;

public interface ILanguageService
{
    Task<List<LanguageDto>> GetAllAsync(CancellationToken ct = default);
    Task<LanguageDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<LanguageDto> CreateAsync(LanguageCreateDto dto, CancellationToken ct = default);
    Task<LanguageDto> UpdateAsync(Guid id, LanguageUpdateDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<LanguageDto> SetDefaultAsync(Guid id, CancellationToken ct = default);
    Task<LanguageDto> ToggleStatusAsync(Guid id, CancellationToken ct = default);
    Task ReorderAsync(List<LanguageReorderItemDto> items, CancellationToken ct = default);
}
