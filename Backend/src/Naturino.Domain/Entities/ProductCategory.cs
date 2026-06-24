using Naturino.Domain.Common;

namespace Naturino.Domain.Entities;

public class ProductCategory : BaseEntity
{
    public Guid? ParentCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameRu { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ImageFileId { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public string? MetaTitleUz { get; set; }
    public string? MetaTitleRu { get; set; }
    public string? MetaDescriptionUz { get; set; }
    public string? MetaDescriptionRu { get; set; }
    public string? MetaKeywords { get; set; }
    public bool IsIndexable { get; set; } = true;
    public bool IsFollow { get; set; } = true;

    public ProductCategory? ParentCategory { get; set; }
    public ICollection<ProductCategory> ChildCategories { get; set; } = new List<ProductCategory>();
    public MediaFile? ImageFile { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
