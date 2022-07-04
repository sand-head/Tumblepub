using System.Text.Json.Serialization;
using Tumblepub.Application.Converters;
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
        
        Id = postCreated.PostId;
        BlogId = postCreated.BlogId;
        Content = postCreated.Content;
        UpdatedAt = CreatedAt = postCreated.At;
    }

    public Post(PostCreated e)
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

[JsonConverter(typeof(PostContentConverter))]
public abstract record PostContent()
{
    public record Internal(string Content) : PostContent()
    {
        public List<string> Tags { get; set; } = new();
    }

    public record External(Uri ExternalId) : PostContent();

    public record Shared(Guid OriginalPostId) : PostContent();

    public record Deleted() : PostContent();
}