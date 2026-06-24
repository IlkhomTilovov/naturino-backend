using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naturino.Domain.Entities;

namespace Naturino.Infrastructure.Persistence.Configurations;

public class BlogConfiguration : IEntityTypeConfiguration<Blog>
{
    public void Configure(EntityTypeBuilder<Blog> builder)
    {
        builder.ToTable("Blogs");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title).HasMaxLength(250).IsRequired();

        builder.Property(b => b.Slug).HasMaxLength(270).IsRequired();
        builder.HasIndex(b => b.Slug).IsUnique();

        builder.Property(b => b.Excerpt).HasMaxLength(500);
        builder.Property(b => b.IsPublished).HasDefaultValue(false);
        builder.Property(b => b.ViewCount).HasDefaultValue(0);
        builder.Property(b => b.IsDeleted).HasDefaultValue(false);

        builder.HasIndex(b => b.CategoryId);

        builder.HasOne(b => b.Category)
            .WithMany(c => c.Blogs)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(b => b.Author)
            .WithMany()
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(b => b.FeaturedImage)
            .WithMany()
            .HasForeignKey(b => b.FeaturedImageId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
