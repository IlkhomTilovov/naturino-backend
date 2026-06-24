using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naturino.Domain.Entities;
using Naturino.Domain.Enums;

namespace Naturino.Infrastructure.Persistence.Configurations;

public class SettingConfiguration : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.ToTable("Settings");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.GroupName).HasMaxLength(100).IsRequired();
        builder.Property(s => s.Key).HasMaxLength(150).IsRequired();

        builder.Property(s => s.ValueType)
            .HasConversion<string>()
            .HasMaxLength(30)
            .HasDefaultValue(SettingValueType.String)
            .IsRequired();

        builder.HasIndex(s => new { s.GroupName, s.Key }).IsUnique();
    }
}
