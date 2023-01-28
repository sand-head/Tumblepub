using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using Tumblepub.Models;

namespace Tumblepub;

public class Subscription
{
    [SubscribeAndResolve]
    public ValueTask<ISourceStream<PostDto>> PostCreated(string blogName, [Service] ITopicEventReceiver receiver)
    {
        var topic = $"{blogName}_{nameof(PostCreated)}";
        return receiver.SubscribeAsync<string, PostDto>(topic);
    }
}