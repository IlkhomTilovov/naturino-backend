using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naturino.Domain.Entities;

namespace Naturino.Infrastructure.Persistence.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("ProductImages");

        builder.HasKey(pi => pi.Id);
        builder.Property(pi => pi.Id).ValueGeneratedNever();

        builder.Property(pi => pi.IsPrimary).HasDefaultValue(false).ValueGeneratedNever();
        builder.Property(pi => pi.SortOrder).HasDefaultValue(0).ValueGeneratedNever();

        builder.HasIndex(pi => pi.ProductId);

        builder.HasOne(pi => pi.Product)
            .WithMany(p => p.Images)
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pi => pi.MediaFile)
            .WithMany()
            .HasForeignKey(pi => pi.MediaFileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
