using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naturino.Domain.Entities;

namespace Naturino.Infrastructure.Persistence.Configurations;

public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        builder.ToTable("Certificates");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title).HasMaxLength(200).IsRequired();
        builder.Property(c => c.IssuedBy).HasMaxLength(200);
        builder.Property(c => c.CertificateNumber).HasMaxLength(100);
        builder.Property(c => c.IssueDate).HasColumnType("date");
        builder.Property(c => c.ExpiryDate).HasColumnType("date");
        builder.Property(c => c.Icon).HasMaxLength(50);
        builder.Property(c => c.Category).HasMaxLength(100);
        builder.Property(c => c.Scope).HasMaxLength(100);
        builder.Property(c => c.VerificationUrl).HasMaxLength(500);
        builder.Property(c => c.TranslationsJson)
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'{}'::jsonb");

        builder.Property(c => c.SortOrder).HasDefaultValue(0);
        builder.Property(c => c.IsActive).HasDefaultValue(true);
        builder.Property(c => c.IsDeleted).HasDefaultValue(false);

        builder.HasOne(c => c.File)
            .WithMany()
            .HasForeignKey(c => c.FileId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
