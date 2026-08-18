using Naturino.Application.Services;
using Naturino.Domain.Exceptions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Naturino.Infrastructure.FileStorage;

public class FileStorageSettings
{
    public string RootPath { get; set; } = "wwwroot/uploads";
    public string PublicBasePath { get; set; } = "/uploads";
}

public class LocalFileStorageService : IFileStorageService
{
    // Uploaded photos routinely come straight off a camera/export at 3000px+
    // and get displayed at a fraction of that size everywhere on the site —
    // capping the long edge here is most of the win for page load time.
    private const int MaxDimension = 2200;

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".pdf"
    };

    private static readonly HashSet<string> ImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    private readonly string _rootPath;
    private readonly string _publicBasePath;

    public LocalFileStorageService(FileStorageSettings settings)
    {
        _rootPath = Path.GetFullPath(settings.RootPath);
        _publicBasePath = settings.PublicBasePath.TrimEnd('/');

        Directory.CreateDirectory(_rootPath);
    }

    public bool IsExtensionAllowed(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return AllowedExtensions.Contains(extension);
    }

    public async Task<StoredFileResult> SaveAsync(Stream content, string originalFileName, string contentType, CancellationToken ct = default)
    {
        if (!IsExtensionAllowed(originalFileName))
        {
            throw new Domain.Exceptions.ValidationException(new Dictionary<string, string[]>
            {
                ["file"] = ["Faqat jpg, jpeg, png, webp, pdf formatlari qo'llab-quvvatlanadi."]
            });
        }

        var extension = Path.GetExtension(originalFileName);
        var safeFileName = $"{Guid.NewGuid():N}{extension}";

        var datedFolder = DateTime.UtcNow.ToString("yyyy/MM");
        var targetDirectory = Path.Combine(_rootPath, datedFolder);
        Directory.CreateDirectory(targetDirectory);

        var fullPath = Path.Combine(targetDirectory, safeFileName);

        long sizeBytes;
        if (ImageExtensions.Contains(extension) && content.CanSeek)
        {
            sizeBytes = await SaveOptimizedImageAsync(content, extension, fullPath, ct);
        }
        else
        {
            await using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await content.CopyToAsync(fileStream, ct);
            sizeBytes = fileStream.Length;
        }

        var relativeUrl = $"{_publicBasePath}/{datedFolder.Replace('\\', '/')}/{safeFileName}";

        return new StoredFileResult
        {
            FileName = safeFileName,
            RelativeUrl = relativeUrl,
            SizeBytes = sizeBytes
        };
    }

    // Resizes oversized photos down to MaxDimension and re-encodes with
    // reasonable compression. Falls back to a raw copy for anything
    // ImageSharp can't decode, so a malformed file never blocks an upload.
    private static async Task<long> SaveOptimizedImageAsync(Stream content, string extension, string fullPath, CancellationToken ct)
    {
        try
        {
            using var image = await Image.LoadAsync(content, ct);

            if (image.Width > MaxDimension || image.Height > MaxDimension)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(MaxDimension, MaxDimension),
                    Sampler = KnownResamplers.Lanczos3
                }));
            }

            await using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                switch (extension.ToLowerInvariant())
                {
                    case ".jpg":
                    case ".jpeg":
                        await image.SaveAsJpegAsync(fileStream, new JpegEncoder { Quality = 82 }, ct);
                        break;
                    case ".webp":
                        await image.SaveAsWebpAsync(fileStream, new WebpEncoder { Quality = 82 }, ct);
                        break;
                    default:
                        await image.SaveAsPngAsync(fileStream, new PngEncoder { CompressionLevel = PngCompressionLevel.BestCompression }, ct);
                        break;
                }
            }

            return new FileInfo(fullPath).Length;
        }
        catch (UnknownImageFormatException)
        {
            content.Seek(0, SeekOrigin.Begin);
            await using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await content.CopyToAsync(fileStream, ct);
            return fileStream.Length;
        }
    }

    public void Delete(string relativeUrl)
    {
        if (!relativeUrl.StartsWith(_publicBasePath, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var relative = relativeUrl[_publicBasePath.Length..].TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_rootPath, relative);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}
