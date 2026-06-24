using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naturino.Domain.Entities;

namespace Naturino.Infrastructure.Persistence.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Code).HasMaxLength(150).IsRequired();
        builder.HasIndex(p => p.Code).IsUnique();

        builder.Property(p => p.Module).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(300);
    }
}
