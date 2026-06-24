using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naturino.Domain.Entities;

namespace Naturino.Infrastructure.Persistence.Configurations;

public class PageSectionConfiguration : IEntityTypeConfiguration<PageSection>
{
    public void Configure(EntityTypeBuilder<PageSection> builder)
    {
        builder.ToTable("PageSections");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.SectionType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.IsEnabled).HasDefaultValue(true);
        builder.Property(s => s.SortOrder).HasDefaultValue(0);

        builder.Property(s => s.Content)
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'{}'::jsonb")
            .IsRequired();

        builder.Property(s => s.DraftContent)
            .HasColumnType("jsonb");

        builder.HasIndex(s => new { s.PageId, s.SortOrder });

        builder.HasOne(s => s.Page)
            .WithMany(p => p.Sections)
            .HasForeignKey(s => s.PageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
