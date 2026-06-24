using System.Text.Json;
using Naturino.Domain.Enums;

namespace Naturino.Application.DTOs.Pages;

public class PageSectionDto
{
    public Guid Id { get; set; }
    public Guid PageId { get; set; }
    public SectionType SectionType { get; set; }
    public int SortOrder { get; set; }
    public bool IsEnabled { get; set; }
    public JsonElement Content { get; set; }
    public bool HasUnpublishedChanges { get; set; }
}

public class UpdatePageSectionDto
{
    public int SortOrder { get; set; }
    public bool IsEnabled { get; set; } = true;
    public JsonElement Content { get; set; }
}

public class CreatePageSectionDto
{
    public SectionType SectionType { get; set; }
    public int SortOrder { get; set; }
    public JsonElement Content { get; set; }
}

public class PageDto
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool IsHomePage { get; set; }
    public bool IsPublished { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public string? OgImageUrl { get; set; }
    public bool IsIndexable { get; set; }
    public bool IsFollow { get; set; }

    public List<PageSectionDto> Sections { get; set; } = [];
}

public class UpdatePageDto
{
    public string Title { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public Guid? OgImageFileId { get; set; }
    public bool IsIndexable { get; set; } = true;
    public bool IsFollow { get; set; } = true;
}

public class CreatePageDto
{
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool IsPublished { get; set; } = true;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public bool IsIndexable { get; set; } = true;
    public bool IsFollow { get; set; } = true;
}
