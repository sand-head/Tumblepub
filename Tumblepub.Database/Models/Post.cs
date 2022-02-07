namespace Tumblepub.Database.Models;

public class Post
{
    public Guid Id { get; set; }
    public Guid BlogId { get; set; }
    public PostContent Content { get; set; } = default!;
}

public abstract record PostContent()
{
    public record Internal() : PostContent();

    public record External(Uri ExternalId) : PostContent();
}