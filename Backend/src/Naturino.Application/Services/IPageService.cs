using Naturino.Application.DTOs.Pages;

namespace Naturino.Application.Services;

public interface IPageService
{
    Task<List<PageDto>> GetAllAsync(CancellationToken ct = default);
    Task<PageDto> CreateAsync(CreatePageDto dto, Guid? userId, CancellationToken ct = default);
    Task<PageDto> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<PageDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PageDto> UpdateAsync(Guid id, UpdatePageDto dto, Guid? userId, CancellationToken ct = default);
    Task<PageSectionDto> UpdateSectionAsync(Guid sectionId, UpdatePageSectionDto dto, CancellationToken ct = default);
    Task<PageSectionDto> AddSectionAsync(Guid pageId, CreatePageSectionDto dto, CancellationToken ct = default);
    Task DeleteSectionAsync(Guid sectionId, CancellationToken ct = default);
    Task<PageDto> PublishAsync(Guid pageId, Guid? userId, CancellationToken ct = default);
}
