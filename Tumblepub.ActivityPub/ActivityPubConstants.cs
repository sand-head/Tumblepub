using Tumblepub.ActivityPub.ActivityStreams;

namespace Tumblepub.ActivityPub;

public static class ActivityPubConstants
{
    public const string ContentType = "application/activity+json";

    /// <summary>
    /// The default route for getting a specific <see cref="Activity"/> done by an <see cref="Actor"/>.
    /// </summary>
    public const string ActorActivityRoute = "/blogs/{0}/activities/{1}";

    /// <summary>
    /// The default route for getting an <see cref="Actor"/>.
    /// </summary>
    public const string ActorRoute = "/blogs/{0}";

    /// <summary>
    /// The default route for getting an <see cref="Actor"/>'s followers list.
    /// </summary>
    public const string ActorFollowersRoute = "/blogs/{0}/followers";

    /// <summary>
    /// The default route for getting a specific <see cref="Object"/> belonging to an <see cref="Actor"/>.
    /// </summary>
    public const string ActorObjectRoute = "/blogs/{0}/posts/{1}";

    /// <summary>
    /// The default route for getting or posting to an <see cref="Actor"/>'s outbox.
    /// </summary>
    public const string ActorOutboxRoute = "/blogs/{0}/outbox";

    /// <summary>
    /// The default route for getting or posting to an <see cref="Actor"/>'s inbox.
    /// </summary>
    public const string ActorInboxRoute = "/blogs/{0}/inbox";
}
