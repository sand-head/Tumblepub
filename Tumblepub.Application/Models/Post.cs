using StronglyTypedIds;
using Tumblepub.Application.Events;

namespace Tumblepub.Application.Models;

[StronglyTypedId(converters: StronglyTypedIdConverter.TypeConverter | StronglyTypedIdConverter.SystemTextJson | StronglyTypedIdConverter.NewtonsoftJson)]
public partial struct PostId { }

public class Post : Aggregate<PostId>
{
    public BlogId BlogId { get; set; }
    public PostContent Content { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    
    public static Post Create(BlogId blogId, PostContent content)
    {
        return new Post(blogId, content);
    }

    private Post(BlogId blogId, PostContent content)
    {
        var postCreated = new PostCreated(PostId.New(), blogId, content, DateTimeOffset.UtcNow);
        
        Enqueue(postCreated);
        Apply(postCreated);
    }

    internal Post(PostCreated e)
    {
        Apply(e);
    }
    
    internal void Apply(PostCreated e)
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

    public abstract record Internal() : PostContent()
    {
        public List<string> Tags { get; set; } = new();
    }

    public record Markdown(string Content) : Internal();
}