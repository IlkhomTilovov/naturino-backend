namespace Naturino.Domain.Entities;

public class MediaFolder
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public Guid? ParentFolderId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public MediaFolder? ParentFolder { get; set; }
    public ICollection<MediaFolder> ChildFolders { get; set; } = new List<MediaFolder>();
    public ICollection<MediaFile> Files { get; set; } = new List<MediaFile>();
}
