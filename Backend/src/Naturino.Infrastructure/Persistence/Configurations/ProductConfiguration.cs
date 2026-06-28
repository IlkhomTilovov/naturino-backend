using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naturino.Domain.Entities;

namespace Naturino.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.SKU).HasMaxLength(64).IsRequired();
        builder.HasIndex(p => p.SKU).IsUnique();

        builder.Property(p => p.Name).HasMaxLength(200).IsRequired();

        builder.Property(p => p.Slug).HasMaxLength(220).IsRequired();
        builder.HasIndex(p => p.Slug).IsUnique();

        builder.Property(p => p.ShortDescription).HasMaxLength(500);
        builder.Property(p => p.NutritionalInfo).HasColumnType("jsonb");
        builder.Property(p => p.PackagingOptions).HasColumnType("jsonb");
        builder.Property(p => p.IngredientsList).HasColumnType("jsonb");
        builder.Property(p => p.Certifications).HasColumnType("jsonb");
        builder.Property(p => p.ExportInfo).HasColumnType("jsonb");
        builder.Property(p => p.Translations).HasColumnType("jsonb");

        builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
        builder.Property(p => p.OldPrice).HasColumnType("decimal(18,2)");
        builder.Property(p => p.Weight).HasColumnType("decimal(10,3)");
        builder.Property(p => p.Brand).HasMaxLength(150);
        builder.Property(p => p.AgeGroup).HasMaxLength(50);
        builder.Property(p => p.StockQuantity).HasDefaultValue(0);

        builder.Property(p => p.IsFeatured).HasDefaultValue(false);
        builder.Property(p => p.IsActive).HasDefaultValue(true);
        builder.Property(p => p.IsDeleted).HasDefaultValue(false);
        builder.Property(p => p.SortOrder).HasDefaultValue(0);

        builder.HasIndex(p => p.CategoryId);

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

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
