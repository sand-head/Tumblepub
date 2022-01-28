namespace Tumblepub.Database.Models;

public class BlogPosts
{
    public Guid Id { get; set; }
    public List<Guid> PostIds { get; set; } = new();
}
