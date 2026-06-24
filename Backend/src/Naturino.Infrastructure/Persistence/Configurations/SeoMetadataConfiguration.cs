using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naturino.Domain.Entities;

namespace Naturino.Infrastructure.Persistence.Configurations;

public class SeoMetadataConfiguration : IEntityTypeConfiguration<SeoMetadata>
{
    public void Configure(EntityTypeBuilder<SeoMetadata> builder)
    {
        builder.ToTable("SEOMetadata");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.EntityType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.MetaTitle).HasMaxLength(200);
        builder.Property(s => s.MetaDescription).HasMaxLength(500);
        builder.Property(s => s.MetaKeywords).HasMaxLength(300);
        builder.Property(s => s.OgTitle).HasMaxLength(200);
        builder.Property(s => s.OgDescription).HasMaxLength(500);
        builder.Property(s => s.CanonicalUrl).HasMaxLength(500);
        builder.Property(s => s.NoIndex).HasDefaultValue(false);
        builder.Property(s => s.StructuredData).HasColumnType("jsonb");

        builder.HasIndex(s => new { s.EntityType, s.EntityId }).IsUnique();

        builder.HasOne(s => s.OgImageFile)
            .WithMany()
            .HasForeignKey(s => s.OgImageFileId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
