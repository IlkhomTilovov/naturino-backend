using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naturino.Domain.Entities;

namespace Naturino.Infrastructure.Persistence.Configurations;

public class MediaFileConfiguration : IEntityTypeConfiguration<MediaFile>
{
    public void Configure(EntityTypeBuilder<MediaFile> builder)
    {
        builder.ToTable("MediaFiles");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.FileName).HasMaxLength(255).IsRequired();
        builder.Property(f => f.OriginalFileName).HasMaxLength(255).IsRequired();
        builder.Property(f => f.Url).HasMaxLength(1000).IsRequired();
        builder.Property(f => f.MimeType).HasMaxLength(100).IsRequired();
        builder.Property(f => f.AltText).HasMaxLength(300);
        builder.Property(f => f.IsDeleted).HasDefaultValue(false);
        builder.Property(f => f.SourceType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(Naturino.Domain.Enums.MediaSourceType.Local);

        builder.HasIndex(f => f.FolderId);

        builder.HasOne(f => f.Folder)
            .WithMany(folder => folder.Files)
            .HasForeignKey(f => f.FolderId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(f => f.UploadedByUser)
            .WithMany()
            .HasForeignKey(f => f.UploadedBy)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
