using Tumblepub.ActivityPub.ActivityStreams;

namespace Tumblepub.ActivityPub;

public class ActivityPubOptions
{
    public string ActorRouteTemplate { get; set; } = ActivityPubConstants.ActorRoute;
    
    public string ActorActivityRouteTemplate { get; set; } = ActivityPubConstants.ActorActivityRoute;

    public string ActorFollowersRouteTemplate { get; set; } = ActivityPubConstants.ActorFollowersRoute;

    public string ActorObjectRouteTemplate { get; set; } = ActivityPubConstants.ActorObjectRoute;

    public string ActorInboxRouteTemplate { get; set; } = ActivityPubConstants.ActorInboxRoute;

    /// <summary>
    /// When this function is set, if a non-ActivityPub request comes in for getting an <see cref="Actor"/>,
    /// it is redirected to the mapped URL.
    /// </summary>
    /// <remarks>
    /// This is intended to be used to redirect users to an <see cref="Actor"/>'s profile page.
    /// </remarks>
    public Func<Actor, string>? MapActorProfileUrl { get; set; }
}
