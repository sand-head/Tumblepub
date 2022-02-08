using Marten.Events.Aggregation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tumblepub.Database.Events;
using Tumblepub.Database.Models;

namespace Tumblepub.Database.Projections;

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
