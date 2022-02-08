using Tumblepub.ActivityPub.ActivityStreams;

namespace Tumblepub.ActivityPub;

public static class ActivityPubConstants
{
    public const string ContentType = "application/activity+json";

    /// <summary>
    /// The default route for getting an <see cref="Actor"/>.
    /// </summary>
    public const string ActorRoute = "/actors/{userId}";

    /// <summary>
    /// The default route for getting a specific <see cref="Activity"/> done by an <see cref="Actor"/>.
    /// </summary>
    public const string ActorActivityRoute = "/actors/{userId}/activities/{activityId}";

    /// <summary>
    /// The default route for getting an <see cref="Actor"/>'s followers list.
    /// </summary>
    public const string ActorFollowersRoute = "/actors/{userId}/followers";

    /// <summary>
    /// The default route for getting a specific <see cref="Object"/> belonging to an <see cref="Actor"/>.
    /// </summary>
    public const string ActorObjectRoute = "/actors/{userId}/objects/{objectId}";

    /// <summary>
    /// The default route for getting or posting to an <see cref="Actor"/>'s inbox.
    /// </summary>
    public const string ActorInboxRoute = "/actors/{userId}/inbox";
}
