namespace Tumblepub.Application.Models;

public class Post
{
    public Guid Id { get; set; }
    public Guid BlogId { get; set; }
    public PostContent Content { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public int Version { get; set; }
}

public abstract record PostContent()
{
    public record External(Uri ExternalId) : PostContent();

    public record Deleted() : PostContent();

    public abstract record Internal() : PostContent()
    {
        public List<string> Tags { get; set; } = new();
    }

    public record Markdown(string Content) : Internal();
}