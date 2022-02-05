using System.Text.Json.Serialization;

namespace Tumblepub.ActivityPub.Models;

public record OrderedCollection() : ActivityPubObject("OrderedCollection")
{
    public int TotalItems { get; set; }
    [JsonPropertyName("next")]
    public Uri NextUrl { get; set; } = default!;
}
