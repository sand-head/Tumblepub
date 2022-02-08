using System.Text.Json.Serialization;

namespace Tumblepub.ActivityPub.ActivityStreams;

public record OrderedCollection() : Object("OrderedCollection")
{
    public int TotalItems { get; init; }
    [JsonPropertyName("next")]
    public Uri NextUrl { get; init; } = default!;
}
