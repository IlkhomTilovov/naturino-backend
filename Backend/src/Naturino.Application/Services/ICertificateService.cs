using Naturino.Application.DTOs.Certificates;

namespace Naturino.Application.Services;

public interface ICertificateService
{
    Task<List<CertificateDto>> GetAllAsync(CancellationToken ct = default);
    Task<CertificateDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CertificateDto> CreateAsync(CertificateCreateDto dto, CancellationToken ct = default);
    Task<CertificateDto> UpdateAsync(Guid id, CertificateUpdateDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<CertificateDto> ToggleStatusAsync(Guid id, CancellationToken ct = default);
    Task ReorderAsync(List<CertificateReorderItemDto> items, CancellationToken ct = default);
}
