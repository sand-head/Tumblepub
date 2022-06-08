using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Tumblepub.Application.Events;
using Tumblepub.Application.Models;

namespace Tumblepub.Infrastructure.Projections;

public class BlogActivityProjection : EventProjection
{
    public BlogActivity Create(PostCreated e)
    {
        return new BlogActivity
        {
            Id = e.PostId,
            BlogId = e.BlogId,
            Type = "Create",
            PublishedAt = e.At,

            ObjectType = new ObjectType.Post(e.PostId),
        };
    }

    public async Task Project(IEvent<PostUpdated> e, IDocumentOperations ops)
    {
        var createActivity = await ops.LoadAsync<BlogActivity>(e.Data.PostId);
        if (createActivity == null)
        {
            return;
        }

        ops.Store(new BlogActivity
        {
            Id = new BlogActivityId(e.Id),
            BlogId = createActivity.BlogId,
            Type = "Update",
            PublishedAt = e.Data.At,

            ObjectType = new ObjectType.Post(e.Data.PostId),
        });
    }
}
