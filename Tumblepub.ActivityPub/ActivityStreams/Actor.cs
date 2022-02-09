using System.Text.Json.Serialization;

namespace Tumblepub.ActivityPub.ActivityStreams;

public record PublicKey
{
    public Uri Id { get; init; } = default!;
    public Uri Owner { get; init; } = default!;
    public string PublicKeyPem { get; init; } = string.Empty;
}

public record Actor(string Type) : Object(Type)
{
    public string Name { get; init; } = string.Empty;
    public string PreferredUsername { get; init; } = string.Empty;
    public string? Summary { get; init; }

    /// <summary>
    /// The inbox URL for the given actor.
    /// </summary>
    /// <remarks>
    /// If relative, the scheme, domain, and port will be added in from the ActivityPub request.
    /// </remarks>
    [JsonPropertyName("inbox")]
    public Uri InboxUrl { get; init; } = default!;

    /// <summary>
    /// The followers URL for the given actor.
    /// </summary>
    /// <remarks>
    /// If relative, the scheme, domain, and port will be added in from the ActivityPub request.
    /// </remarks>
    [JsonPropertyName("followers")]
    public Uri FollowersUrl { get; init; } = default!;

    public PublicKey PublicKey { get; init; } = default!;
}
