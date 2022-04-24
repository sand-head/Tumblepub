using Marten.Events.Aggregation;
using Tumblepub.Application.Events;
using Tumblepub.Application.Models;

namespace Tumblepub.Infrastructure.Projections;

public class PostProjection : AggregateProjection<Post>
{
    public Post Create(PostCreated e)
    {
        return new Post
        {
            Id = e.PostId,
            BlogId = e.BlogId,
            Content = e.Content,
            UpdatedAt = e.At,
            CreatedAt = e.At
        };
    }

    public void Apply(PostDeleted e, Post post)
    {
        post.Content = new PostContent.Deleted();
        post.UpdatedAt = e.At;
        post.Version++;
    }
}
