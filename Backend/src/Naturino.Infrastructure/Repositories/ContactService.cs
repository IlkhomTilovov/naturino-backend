using Naturino.Application.DTOs.Contacts;
using Naturino.Application.Services;
using Naturino.Domain.Entities;
using Naturino.Infrastructure.Persistence;

namespace Naturino.Infrastructure.Repositories;

public class ContactService : IContactService
{
    private readonly ApplicationDbContext _context;

    public ContactService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ContactDto> SubmitAsync(CreateContactDto dto, string? ipAddress, CancellationToken ct = default)
    {
        var contact = new Contact
        {
            Type = dto.Type,
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            Company = dto.Company,
            Subject = dto.Subject,
            Message = dto.Message,
            IpAddress = ipAddress,
        };

        _context.Contacts.Add(contact);
        await _context.SaveChangesAsync(ct);

        return new ContactDto
        {
            Id = contact.Id,
            Type = contact.Type,
            Name = contact.Name,
            Email = contact.Email,
            Phone = contact.Phone,
            Company = contact.Company,
            Subject = contact.Subject,
            Message = contact.Message,
            Status = contact.Status,
            SubmittedAt = contact.SubmittedAt,
        };
    }
}
