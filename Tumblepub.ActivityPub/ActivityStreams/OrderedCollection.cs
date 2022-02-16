using System.Text.Json.Serialization;

namespace Tumblepub.ActivityPub.ActivityStreams;

public record OrderedCollection() : Object(Types[0])
{
    public static new readonly string[] Types = new[] { "OrderedCollection" };

    public int TotalItems { get; init; }
    [JsonPropertyName("first")]
    public Uri FirstUrl { get; init; } = default!;
}

public record OrderedCollection<T>() : Object(Types[0])
{
    public static new readonly string[] Types = new[] { "OrderedCollectionPage" };

    [JsonPropertyName("next")]
    public Uri NextUrl { get; init; } = default!;
    [JsonPropertyName("prev")]
    public Uri PreviousUrl { get; init; } = default!;
    [JsonPropertyName("partOf")]
    public Uri PartOfUrl { get; init; } = default!;
    public List<T> OrderedItems { get; init; } = new();
}