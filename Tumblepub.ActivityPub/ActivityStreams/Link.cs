using System.Text.Json.Serialization;
using Tumblepub.ActivityPub.Converters;

namespace Tumblepub.ActivityPub.ActivityStreams;

public record Link(Uri Href) : ActivityStreamsValue("Link")
{
    public static readonly string[] Types = new[] { "Link", "Mention" };

    [JsonConverter(typeof(MaybeSingleMaybeArrayConverterFactory))]
    public IEnumerable<string>? Rel { get; init; }
    public string? MediaType { get; init; }
    public string? Name { get; init; }
    public string? Hreflang { get; init; }
    public int? Height { get; init; }
    public int? Width { get; init; }
    public Uri? Preview { get; init; }
}
