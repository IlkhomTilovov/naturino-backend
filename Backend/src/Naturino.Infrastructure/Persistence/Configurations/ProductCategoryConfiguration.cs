using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naturino.Domain.Entities;

namespace Naturino.Infrastructure.Persistence.Configurations;

public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.ToTable("ProductCategories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).HasMaxLength(150).IsRequired();
        builder.Property(c => c.NameRu).HasMaxLength(150);

        builder.Property(c => c.Slug).HasMaxLength(170).IsRequired();
        builder.HasIndex(c => c.Slug).IsUnique();

        builder.Property(c => c.IsActive).HasDefaultValue(true);
        builder.Property(c => c.SortOrder).HasDefaultValue(0);

        builder.Property(c => c.MetaTitleUz).HasMaxLength(60);
        builder.Property(c => c.MetaTitleRu).HasMaxLength(60);
        builder.Property(c => c.MetaDescriptionUz).HasMaxLength(160);
        builder.Property(c => c.MetaDescriptionRu).HasMaxLength(160);
        builder.Property(c => c.MetaKeywords).HasMaxLength(300);
        builder.Property(c => c.IsIndexable).HasDefaultValue(true);
        builder.Property(c => c.IsFollow).HasDefaultValue(true);
        builder.Property(c => c.Translations).HasColumnType("jsonb");

        builder.HasIndex(c => c.SortOrder);

        builder.HasOne(c => c.ParentCategory)
            .WithMany(c => c.ChildCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(c => c.ImageFile)
            .WithMany()
            .HasForeignKey(c => c.ImageFileId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
