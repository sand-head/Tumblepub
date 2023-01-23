using Tumblepub.ActivityPub.ActivityStreams;
using Tumblepub.Application.Aggregates;

namespace Tumblepub.ActivityPub.Extensions;

internal static class PostExtensions
{
    public static ActivityStreamsValue ToObject(this Post post)
    {
        if (post.Content is PostContent.External externalContent)
        {
            return new Link(externalContent.ExternalId);
        }

        // I should simplify how these objects are constructed a bit...
        var postObject = new ActivityStreamsObject("Note")
        {
            Context = new List<string>()
            {
                "https://www.w3.org/ns/activitystreams"
            },

            Id = new(string.Format(ActivityPubConstants.ActorObjectRoute, post.BlogId, post.Id), UriKind.Relative),
            PublishedAt = post.CreatedAt,
            AttributedTo = new()
            {
                new Link(new(string.Format(ActivityPubConstants.ActorRoute, post.BlogId), UriKind.Relative))
            },

            To = new()
            {
                new Link(new("https://www.w3.org/ns/activitystreams#Public"))
            }
        };

        return post.Content switch
        {
            PostContent.Internal textContent => postObject with
            {
                Content = textContent.Content
            },
            _ => postObject
        };
    }
}