using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using Tumblepub.Models;

namespace Tumblepub;

public class Subscription
{
    public ValueTask<ISourceStream<PostDto>> SubscribeToCreatedPosts(string blogName, ITopicEventReceiver receiver)
    {
        var topic = $"{blogName}_CreatedPost";
        return receiver.SubscribeAsync<PostDto>(topic);
    }

    [Subscribe(With = nameof(SubscribeToCreatedPosts))]
    public PostDto PostCreated(string blogName, [EventMessage] PostDto postDto) => postDto;
}