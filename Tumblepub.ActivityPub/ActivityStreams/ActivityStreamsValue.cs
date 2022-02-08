using System.Text.Json.Serialization;
using Tumblepub.ActivityPub.Converters;

namespace Tumblepub.ActivityPub.ActivityStreams;

public abstract record ActivityStreamsValue(string Type)
{
    [JsonPropertyName("@context"), JsonConverter(typeof(MaybeSingleMaybeArrayConverterFactory))]
    public IEnumerable<string> Context { get; init; } = new List<string>();

    /// <summary>
    /// Provides the globally unique identifier for an <see cref="ActivityStreamsValue"/>.
    /// </summary>
    /// <remarks>
    /// If relative, the scheme, domain, and port will be added in from the ActivityPub request.
    /// </remarks>
    public Uri? Id { get; init; } = default!;
}
