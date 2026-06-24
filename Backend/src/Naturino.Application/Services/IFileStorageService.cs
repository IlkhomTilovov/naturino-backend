namespace Naturino.Application.Services;

public class StoredFileResult
{
    public string FileName { get; set; } = string.Empty;
    public string RelativeUrl { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
}

public interface IFileStorageService
{
    bool IsExtensionAllowed(string fileName);
    Task<StoredFileResult> SaveAsync(Stream content, string originalFileName, string contentType, CancellationToken ct = default);
    void Delete(string relativeUrl);
}
