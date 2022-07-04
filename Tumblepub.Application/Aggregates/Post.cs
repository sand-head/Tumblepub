using Tumblepub.Application.Events;

namespace Tumblepub.Application.Aggregates;

public class Post : Aggregate<Guid>
{
    public Guid BlogId { get; set; }
    public PostContent Content { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    
    public Post() { }

    public Post(Guid blogId, PostContent content)
    {
        var postCreated = new PostCreated(Guid.NewGuid(), blogId, content, DateTimeOffset.UtcNow);
        
        Enqueue(postCreated);
        Initialize(postCreated);
    }

    public Post(PostCreated e)
    {
        Initialize(e);
    }
    
    private void Initialize(PostCreated e)
    {
        Id = e.PostId;
        BlogId = e.BlogId;
        Content = e.Content;
        UpdatedAt = CreatedAt = e.At;
    }
    internal void Apply(PostDeleted e)
    {
        Content = new PostContent.Deleted();
        UpdatedAt = e.At;
        Version++;
    }
}

public abstract record PostContent()
{
    public record External(Uri ExternalId) : PostContent();

    public record Deleted() : PostContent();

    public record Shared(Guid OriginalPostId) : PostContent();

    public record Markdown(string Content) : PostContent()
    {
        public List<string> Tags { get; set; } = new();
    }
}