using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naturino.Domain.Entities;

namespace Naturino.Infrastructure.Persistence.Configurations;

public class PageConfiguration : IEntityTypeConfiguration<Page>
{
    public void Configure(EntityTypeBuilder<Page> builder)
    {
        builder.ToTable("Pages");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Slug).HasMaxLength(200).IsRequired();
        builder.HasIndex(p => p.Slug).IsUnique();

        builder.Property(p => p.Title).HasMaxLength(200).IsRequired();
        builder.Property(p => p.IsHomePage).HasDefaultValue(false);
        builder.Property(p => p.IsPublished).HasDefaultValue(false);
        builder.Property(p => p.IsDeleted).HasDefaultValue(false);

        builder.Property(p => p.MetaTitle).HasMaxLength(60);
        builder.Property(p => p.MetaDescription).HasMaxLength(160);
        builder.Property(p => p.MetaKeywords).HasMaxLength(300);
        builder.Property(p => p.IsIndexable).HasDefaultValue(true);
        builder.Property(p => p.IsFollow).HasDefaultValue(true);

        builder.HasOne(p => p.OgImageFile)
            .WithMany()
            .HasForeignKey(p => p.OgImageFileId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(p => p.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(p => p.UpdatedBy)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
