using Tumblepub.ActivityPub.ActivityStreams;
using Tumblepub.Application.Aggregates;
using Tumblepub.Application.Interfaces;

namespace Tumblepub.ActivityPub.Extensions;

internal static class BlogActivityExtensions
{
    public static async Task<Activity> ToActivityAsync(
        this BlogActivity blogActivity,
        IReadOnlyRepository<Blog, Guid> blogRepository,
        IReadOnlyRepository<Post, Guid> postRepository,
        CancellationToken token = default)
    {
        return new(blogActivity.Type)
        {
            Context = new List<string>()
            {
                "https://www.w3.org/ns/activitystreams"
            },

            Id = new(string.Format(ActivityPubConstants.ActorActivityRoute, blogActivity.BlogId, blogActivity.Id), UriKind.Relative),
            Actor = new List<ActivityStreamsValue>
            {
                new Link(new(string.Format(ActivityPubConstants.ActorRoute, blogActivity.BlogId), UriKind.Relative))
            },
            PublishedAt = blogActivity.PublishedAt,

            Object = blogActivity.ObjectType switch
            {
                // maybe this kind of sucks and I should just make these links
                ObjectType.Blog => (await blogRepository.GetByIdAsync(blogActivity.ObjectId!.Value, token))!.ToActor(),
                ObjectType.Post => (await postRepository.GetByIdAsync(blogActivity.ObjectId!.Value, token))!.ToObject(),
                _ => null
            },
        };
    }
}