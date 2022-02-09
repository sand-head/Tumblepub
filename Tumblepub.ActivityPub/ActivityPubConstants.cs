using Tumblepub.ActivityPub.ActivityStreams;

namespace Tumblepub.ActivityPub;

public static class ActivityPubConstants
{
    public const string ContentType = "application/activity+json";

    /// <summary>
    /// The default route for getting a specific <see cref="Activity"/> done by an <see cref="Actor"/>.
    /// </summary>
    public const string ActorActivityRoute = "/actors/{0}/activities/{1}";

    /// <summary>
    /// The default route for getting an <see cref="Actor"/>.
    /// </summary>
    public const string ActorRoute = "/actors/{0}";

    /// <summary>
    /// The default route for getting an <see cref="Actor"/>'s followers list.
    /// </summary>
    public const string ActorFollowersRoute = "/actors/{0}/followers";

    /// <summary>
    /// The default route for getting a specific <see cref="Object"/> belonging to an <see cref="Actor"/>.
    /// </summary>
    public const string ActorObjectRoute = "/actors/{0}/objects/{1}";

    /// <summary>
    /// The default route for getting or posting to an <see cref="Actor"/>'s outbox.
    /// </summary>
    public const string ActorOutboxRoute = "/actors/{0}/outbox";

    /// <summary>
    /// The default route for getting or posting to an <see cref="Actor"/>'s inbox.
    /// </summary>
    public const string ActorInboxRoute = "/actors/{0}/inbox";
}
