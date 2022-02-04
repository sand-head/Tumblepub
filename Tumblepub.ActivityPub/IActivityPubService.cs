using Tumblepub.ActivityPub.Models;

namespace Tumblepub.ActivityPub;

public interface IActivityPubService
{
    Task<Actor?> GetActor(Guid id, CancellationToken token = default);
    Task GetActorFollowers(Guid id, CancellationToken token = default);
}
