using Marten.Events.Aggregation;
using System.Text.Json;
using Tumblepub.Events;

namespace Tumblepub.Projections;

public class Blog
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string BlogName { get; set; } = default!;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public JsonElement? Metadata { get; set; }
    public string PublicKey { get; set; } = default!;
    public string? PrivateKey { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public int Version { get; set; }
}

public class BlogProjection : AggregateProjection<Blog>
{
    public BlogProjection()
    {
        DeleteEvent<BlogDeleted>();
    }

    public Blog Create(BlogCreated e)
    {
        return new Blog
        {
            Id = e.BlogId,
            UserId = e.UserId,
            BlogName = e.BlogName,
            PublicKey = e.PublicKey,
            PrivateKey = e.PrivateKey,
            UpdatedAt = e.At,
            CreatedAt = e.At
        };
    }

    public Blog Create(BlogDiscovered e)
    {
        return new Blog
        {
            Id = e.BlogId,
            BlogName = e.BlogName,
            PublicKey = e.PublicKey,
            UpdatedAt = e.At,
            CreatedAt = e.At
        };
    }

    public void Apply(BlogMetadataUpdated e, Blog blog)
    {
        blog.Title = e.Title ?? blog.Title;
        blog.Description = e.Description ?? blog.Description;
        blog.Metadata = e.Metadata ?? blog.Metadata;
        blog.Version++;
    }
}