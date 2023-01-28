using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using Tumblepub.Application.Events;

namespace Tumblepub;

public class Subscription
{
    [SubscribeAndResolve]
    public ValueTask<ISourceStream<PostCreated>> PostCreated(Guid blogId, [Service] ITopicEventReceiver receiver)
    {
        var topic = $"{blogId}_{nameof(PostCreated)}";
        return receiver.SubscribeAsync<string, PostCreated>(topic);
    }
}