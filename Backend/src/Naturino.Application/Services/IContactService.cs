using Naturino.Application.DTOs.Contacts;

namespace Naturino.Application.Services;

public interface IContactService
{
    Task<ContactDto> SubmitAsync(CreateContactDto dto, string? ipAddress, CancellationToken ct = default);
}
