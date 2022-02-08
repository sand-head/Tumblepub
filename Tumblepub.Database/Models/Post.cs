using Marten.Schema;

namespace Tumblepub.Database.Models;

public class Post
{
    public Guid Id { get; set; }
    [ForeignKey(typeof(Blog))]
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

    public record Internal() : PostContent()
    {
        public List<string> Tags { get; set; } = new();
    }

    public record Text(string Content) : Internal();
}