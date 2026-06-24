using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naturino.Domain.Entities;

namespace Naturino.Infrastructure.Persistence.Configurations;

public class BlogCategoryConfiguration : IEntityTypeConfiguration<BlogCategory>
{
    public void Configure(EntityTypeBuilder<BlogCategory> builder)
    {
        builder.ToTable("BlogCategories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).HasMaxLength(150).IsRequired();

        builder.Property(c => c.Slug).HasMaxLength(170).IsRequired();
        builder.HasIndex(c => c.Slug).IsUnique();
    }
}
