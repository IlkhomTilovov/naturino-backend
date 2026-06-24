namespace Naturino.Domain.Entities;

public class BlogCategory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
}
