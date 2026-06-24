using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naturino.Domain.Entities;

namespace Naturino.Infrastructure.Persistence.Configurations;

public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable("Languages");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Name).HasMaxLength(100).IsRequired();
        builder.Property(l => l.NativeName).HasMaxLength(100).IsRequired();

        builder.Property(l => l.Code).HasMaxLength(10).IsRequired();
        builder.HasIndex(l => l.Code).IsUnique();

        builder.Property(l => l.Locale).HasMaxLength(20).IsRequired();
        builder.HasIndex(l => l.Locale).IsUnique();

        builder.Property(l => l.Flag).HasMaxLength(10);
        builder.Property(l => l.Direction).HasMaxLength(3).HasDefaultValue("ltr");

        builder.Property(l => l.IsDefault).HasDefaultValue(false);
        builder.Property(l => l.IsActive).HasDefaultValue(true);
        builder.Property(l => l.SortOrder).HasDefaultValue(0);

        builder.HasIndex(l => l.SortOrder);
    }
}
