using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naturino.Domain.Entities;

namespace Naturino.Infrastructure.Persistence.Configurations;

public class ThemeConfiguration : IEntityTypeConfiguration<Theme>
{
    public void Configure(EntityTypeBuilder<Theme> builder)
    {
        builder.ToTable("Themes");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).HasMaxLength(150).IsRequired();
        builder.Property(t => t.Slug).HasMaxLength(170).IsRequired();
        builder.HasIndex(t => t.Slug).IsUnique();

        builder.Property(t => t.FontHeading).HasMaxLength(100);
        builder.Property(t => t.FontBody).HasMaxLength(100);

        builder.Property(t => t.ColorTokensJson).HasColumnType("jsonb");
        builder.Property(t => t.TypographyTokensJson).HasColumnType("jsonb");
        builder.Property(t => t.RadiusTokensJson).HasColumnType("jsonb");
        builder.Property(t => t.ShadowTokensJson).HasColumnType("jsonb");
        builder.Property(t => t.ButtonTokensJson).HasColumnType("jsonb");
        builder.Property(t => t.BrandingTokensJson).HasColumnType("jsonb");
        builder.Property(t => t.LayoutTokensJson).HasColumnType("jsonb");
        builder.Property(t => t.AnimationTokensJson).HasColumnType("jsonb");
        builder.Property(t => t.CustomCss).HasColumnType("text");

        builder.Property(t => t.Description).HasMaxLength(500);
        builder.Property(t => t.Version).HasMaxLength(20);
        builder.Property(t => t.AppearanceMode).HasMaxLength(20);

        builder.Property(t => t.IsActive).HasDefaultValue(false);
        builder.Property(t => t.IsDarkMode).HasDefaultValue(false);
    }
}
