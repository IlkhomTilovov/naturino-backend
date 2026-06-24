namespace Naturino.Domain.Entities;

public class ProductImage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }
    public Guid MediaFileId { get; set; }
    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }

    public Product Product { get; set; } = null!;
    public MediaFile MediaFile { get; set; } = null!;
}
