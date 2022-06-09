using System.ComponentModel;
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
        var postId = GetPostIdGuid(e.PostId);
        return new BlogActivity(new BlogActivityId(postId))
        {
            BlogId = e.BlogId,
            Type = "Create",
            PublishedAt = e.At,

            ObjectType = new ObjectType.Post(e.PostId),
        };
    }

    public async Task Project(IEvent<PostUpdated> e, IDocumentOperations ops)
    {
        var createActivity = await ops.LoadAsync<BlogActivity>(GetPostIdGuid(e.Data.PostId));
        if (createActivity == null)
        {
            return;
        }

        ops.Store(new BlogActivity(new BlogActivityId(e.Id))
        {
            BlogId = createActivity.BlogId,
            Type = "Update",
            PublishedAt = e.Data.At,

            ObjectType = new ObjectType.Post(e.Data.PostId),
        });
    }
    
    private Guid GetPostIdGuid(PostId id)
    {
        return (Guid)TypeDescriptor.GetConverter(typeof(PostId)).ConvertTo(id, typeof(Guid))!;
    }
}
