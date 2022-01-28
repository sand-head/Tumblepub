namespace Tumblepub.Database.Models;

public class UserBlogs
{
    public Guid Id { get; set; }
    public List<Guid> BlogIds { get; set; } = new();
}
