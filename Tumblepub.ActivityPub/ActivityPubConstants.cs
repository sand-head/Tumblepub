using Tumblepub.ActivityPub.ActivityStreams;

namespace Tumblepub.ActivityPub;

public static class ActivityPubConstants
{
    /// <summary>
    /// The default route for getting an <see cref="Actor"/>.
    /// </summary>
    public const string ActorRoute = "/actors/{userId}";

    /// <summary>
    /// The default route for getting an <see cref="Actor"/>'s followers list.
    /// </summary>
    public const string ActorFollwersRoute = "/actors/{userId}/followers";

    /// <summary>
    /// The default route for getting or posting to an <see cref="Actor"/>'s inbox.
    /// </summary>
    public const string ActorInboxRoute = "/actors/{userId}/inbox";
}
