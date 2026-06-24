using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naturino.Domain.Entities;

namespace Naturino.Infrastructure.Persistence.Configurations;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("Contacts");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Type)
            .HasConversion<string>()
            .HasMaxLength(30)
            .HasDefaultValue(Domain.Enums.ContactType.General)
            .IsRequired();

        builder.Property(c => c.Name).HasMaxLength(150).IsRequired();
        builder.Property(c => c.Email).HasMaxLength(256).IsRequired();
        builder.Property(c => c.Phone).HasMaxLength(50);
        builder.Property(c => c.Company).HasMaxLength(200);
        builder.Property(c => c.Subject).HasMaxLength(250);
        builder.Property(c => c.Message).IsRequired();
        builder.Property(c => c.IpAddress).HasMaxLength(64);

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .HasDefaultValue(Domain.Enums.ContactStatus.New)
            .IsRequired();

        builder.HasIndex(c => c.Status);
    }
}
