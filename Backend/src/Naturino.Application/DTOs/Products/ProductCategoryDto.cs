namespace Naturino.Application.DTOs.Products;

public class ProductCategoryDto
{
    public Guid Id { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameRu { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public int ProductCount { get; set; }

    public string? MetaTitleUz { get; set; }
    public string? MetaTitleRu { get; set; }
    public string? MetaDescriptionUz { get; set; }
    public string? MetaDescriptionRu { get; set; }
    public string? MetaKeywords { get; set; }
    public bool IsIndexable { get; set; }
    public bool IsFollow { get; set; }
}

public class ProductCategoryCreateDto
{
    public Guid? ParentCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameRu { get; set; }
    public string? Slug { get; set; }
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
}

public class ProductCategoryUpdateDto : ProductCategoryCreateDto
{
}

public class ProductCategoryReorderItemDto
{
    public Guid Id { get; set; }
    public int SortOrder { get; set; }
}
