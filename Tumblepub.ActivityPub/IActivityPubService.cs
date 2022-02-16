using Tumblepub.ActivityPub.ActivityStreams;

namespace Tumblepub.ActivityPub;

public interface IActivityPubService
{
    Task<Actor?> GetActor(Guid id, CancellationToken token = default);
    Task<Actor?> GetActorByName(string name, CancellationToken token = default);
    Task<Activity?> GetActivity(Guid actorId, Guid activityId, CancellationToken token = default);
    Task<ActivityStreams.Object?> GetObject(Guid actorId, Guid objectId, CancellationToken token = default);
    Task<ActivityStreams.Object?> GetOutbox(Guid actorId, int? pageNumber = null, CancellationToken token = default);
    //Task GetActorFollowers(Guid id, CancellationToken token = default);
}
