namespace Tumblepub.Database.Models;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public List<Guid> Blogs { get; set; } = new();
}
