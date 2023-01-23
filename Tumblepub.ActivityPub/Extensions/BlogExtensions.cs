using Tumblepub.ActivityPub.ActivityStreams;
using Tumblepub.Application.Aggregates;

namespace Tumblepub.ActivityPub.Extensions;

internal static class BlogExtensions
{
    public static Actor ToActor(this Blog blog)
    {
        var actorId = string.Format(ActivityPubConstants.ActorRoute, blog.Id);
        return new("Person")
        {
            Context = new List<string>()
            {
                "https://www.w3.org/ns/activitystreams"
            },

            // todo: get domain
            Id = new(actorId, UriKind.Relative),
            Name = blog.Name,
            PublishedAt = blog.CreatedAt,
            PreferredUsername = blog.Title ?? blog.Name,
            Summary = blog.Description,

            InboxUrl = new(string.Format(ActivityPubConstants.ActorInboxRoute, blog.Id), UriKind.Relative),
            FollowersUrl = new(string.Format(ActivityPubConstants.ActorOutboxRoute, blog.Id), UriKind.Relative),

            PublicKey = new()
            {
                Id = new($"{actorId}#main-key", UriKind.Relative),
                Owner = new(actorId, UriKind.Relative),
                PublicKeyPem = blog.PublicKey
            }
        };
    }
}