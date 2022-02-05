using System.Text.Json.Serialization;
using Tumblepub.ActivityPub.Converters;

namespace Tumblepub.ActivityPub.Models;

public abstract record ActivityPubObject(string Type)
{
    [JsonPropertyName("@context"), JsonConverter(typeof(MaybeStringMaybeArrayConverter))]
    public List<string> Context { get; set; } = new();

    /// <summary>
    /// The ID for the given <see cref="ActivityPubObject"/>. This should be the URL used to access this object.
    /// </summary>
    /// <remarks>
    /// If relative, the scheme, domain, and port will be added in from the ActivityPub request.
    /// </remarks>
    public Uri Id { get; set; } = default!;
}
