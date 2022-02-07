using Tumblepub.ActivityPub.ActivityStreams;

namespace Tumblepub.ActivityPub;

public interface IActivityPubService
{
    Task<Actor?> GetActor(Guid id, CancellationToken token = default);
    Task<Actor?> GetActorByName(string name, CancellationToken token = default);
    Task GetActorFollowers(Guid id, CancellationToken token = default);
}
