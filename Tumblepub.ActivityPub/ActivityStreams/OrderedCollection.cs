using System.Text.Json.Serialization;

namespace Tumblepub.ActivityPub.ActivityStreams;

public record OrderedCollection() : Object("OrderedCollection")
{
    public int TotalItems { get; set; }
    [JsonPropertyName("next")]
    public Uri NextUrl { get; set; } = default!;
}
