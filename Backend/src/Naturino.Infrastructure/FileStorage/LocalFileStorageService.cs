using Naturino.Application.Services;
using Naturino.Domain.Exceptions;

namespace Naturino.Infrastructure.FileStorage;

public class FileStorageSettings
{
    public string RootPath { get; set; } = "wwwroot/uploads";
    public string PublicBasePath { get; set; } = "/uploads";
}

public class LocalFileStorageService : IFileStorageService
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
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
                ["file"] = ["Faqat jpg, jpeg, png, webp formatlari qo'llab-quvvatlanadi."]
            });
        }

        var extension = Path.GetExtension(originalFileName);
        var safeFileName = $"{Guid.NewGuid():N}{extension}";

        var datedFolder = DateTime.UtcNow.ToString("yyyy/MM");
        var targetDirectory = Path.Combine(_rootPath, datedFolder);
        Directory.CreateDirectory(targetDirectory);

        var fullPath = Path.Combine(targetDirectory, safeFileName);

        await using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await content.CopyToAsync(fileStream, ct);

        var relativeUrl = $"{_publicBasePath}/{datedFolder.Replace('\\', '/')}/{safeFileName}";

        return new StoredFileResult
        {
            FileName = safeFileName,
            RelativeUrl = relativeUrl,
            SizeBytes = fileStream.Length
        };
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
