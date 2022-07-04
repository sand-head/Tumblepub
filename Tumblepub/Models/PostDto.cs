namespace Tumblepub.Models;

[GraphQLName("Post")]
public class PostDto
{
    public Guid Id { get; set; }
    [GraphQLIgnore]
    public Guid BlogId { get; set; }
    public string Content { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}