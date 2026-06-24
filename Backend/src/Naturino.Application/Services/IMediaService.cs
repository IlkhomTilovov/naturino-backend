using Naturino.Application.DTOs.Media;

namespace Naturino.Application.Services;

public interface IMediaService
{
    Task<MediaFileDto> UploadAsync(Stream content, string originalFileName, string contentType, long sizeBytes, Guid? uploadedBy, CancellationToken ct = default);
    Task<MediaFileDto> CreateFromUrlAsync(CreateMediaFromUrlDto dto, Guid? uploadedBy, CancellationToken ct = default);
    Task<List<MediaFileDto>> GetAllAsync(CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
