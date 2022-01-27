namespace Tumblepub.Database.Models;

public class BlogDto
{
    public string Name { get; set; } = default!;
    public string? Title { get; set; }
    public string? Description { get; set; }
}
