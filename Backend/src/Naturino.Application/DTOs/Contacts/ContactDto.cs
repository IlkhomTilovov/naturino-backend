using Naturino.Domain.Enums;

namespace Naturino.Application.DTOs.Contacts;

public class CreateContactDto
{
    public ContactType Type { get; set; } = ContactType.General;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public string? Subject { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class ContactDto
{
    public Guid Id { get; set; }
    public ContactType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public string? Subject { get; set; }
    public string Message { get; set; } = string.Empty;
    public ContactStatus Status { get; set; }
    public DateTimeOffset SubmittedAt { get; set; }
}
