namespace Tumblepub.Database.Models;

public class UserBlogs
{
    public string UserEmail { get; set; } = string.Empty;
    public List<Guid> Blogs { get; set; } = new();
}
