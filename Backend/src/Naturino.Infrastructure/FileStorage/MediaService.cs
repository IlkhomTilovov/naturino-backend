using Microsoft.EntityFrameworkCore;
using Naturino.Application.DTOs.Media;
using Naturino.Application.Services;
using Naturino.Domain.Entities;
using Naturino.Domain.Enums;
using Naturino.Domain.Exceptions;
using Naturino.Infrastructure.Persistence;

namespace Naturino.Infrastructure.FileStorage;

public class MediaService : IMediaService
{
    private readonly ApplicationDbContext _context;
    private readonly IFileStorageService _fileStorageService;

    public MediaService(ApplicationDbContext context, IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    public async Task<MediaFileDto> UploadAsync(Stream content, string originalFileName, string contentType, long sizeBytes, Guid? uploadedBy, CancellationToken ct = default)
    {
        var stored = await _fileStorageService.SaveAsync(content, originalFileName, contentType, ct);

        var mediaFile = new MediaFile
        {
            FileName = stored.FileName,
            OriginalFileName = originalFileName,
            Url = stored.RelativeUrl,
            MimeType = contentType,
            FileSizeBytes = stored.SizeBytes,
            SourceType = MediaSourceType.Local,
            UploadedBy = uploadedBy
        };

        _context.MediaFiles.Add(mediaFile);
        await _context.SaveChangesAsync(ct);

        return ToDto(mediaFile);
    }

    public async Task<MediaFileDto> CreateFromUrlAsync(CreateMediaFromUrlDto dto, Guid? uploadedBy, CancellationToken ct = default)
    {
        if (!Uri.TryCreate(dto.Url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new Domain.Exceptions.ValidationException(new Dictionary<string, string[]>
            {
                ["url"] = ["To'g'ri http(s) URL kiriting."]
            });
        }

        var fileName = Path.GetFileName(uri.AbsolutePath);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            fileName = $"external-{Guid.NewGuid():N}";
        }

        var mediaFile = new MediaFile
        {
            FileName = fileName,
            OriginalFileName = fileName,
            Url = dto.Url,
            MimeType = "image/external",
            FileSizeBytes = 0,
            SourceType = MediaSourceType.ExternalUrl,
            AltText = dto.AltText,
            UploadedBy = uploadedBy
        };

        _context.MediaFiles.Add(mediaFile);
        await _context.SaveChangesAsync(ct);

        return ToDto(mediaFile);
    }

    public async Task<List<MediaFileDto>> GetAllAsync(CancellationToken ct = default)
    {
        var files = await _context.MediaFiles
            .Where(f => !f.IsDeleted)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(ct);

        return files.Select(ToDto).ToList();
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var file = await _context.MediaFiles.FirstOrDefaultAsync(f => f.Id == id, ct)
            ?? throw new NotFoundException(nameof(MediaFile), id);

        file.IsDeleted = true;
        file.DeletedAt = DateTimeOffset.UtcNow;

        if (file.SourceType == MediaSourceType.Local)
        {
            _fileStorageService.Delete(file.Url);
        }

        await _context.SaveChangesAsync(ct);
    }

    private static MediaFileDto ToDto(MediaFile file) => new()
    {
        Id = file.Id,
        FileName = file.FileName,
        OriginalFileName = file.OriginalFileName,
        Url = file.Url,
        MimeType = file.MimeType,
        SourceType = file.SourceType,
        FileSizeBytes = file.FileSizeBytes,
        AltText = file.AltText,
        CreatedAt = file.CreatedAt
    };
}
