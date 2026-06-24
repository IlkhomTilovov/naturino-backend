using Naturino.Domain.Enums;

namespace Naturino.Domain.Entities;

public class Contact
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public ContactType Type { get; set; } = ContactType.General;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public string? Subject { get; set; }
    public string Message { get; set; } = string.Empty;
    public ContactStatus Status { get; set; } = ContactStatus.New;
    public string? IpAddress { get; set; }
    public DateTimeOffset SubmittedAt { get; set; } = DateTimeOffset.UtcNow;
}
