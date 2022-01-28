using Marten.Events.Aggregation;
using Tumblepub.Database.Events;
using Tumblepub.Database.Models;

namespace Tumblepub.Database.Projections;

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
            Name = e.BlogName,
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
            Name = e.BlogName,
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