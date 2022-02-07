using System.Text.Json.Serialization;
using Tumblepub.ActivityPub.Converters;

namespace Tumblepub.ActivityPub.ActivityStreams;

[JsonConverter(typeof(LinkConverter))]
public record Link(Uri Href) : ActivityStreamsValue("Link")
{
    [JsonConverter(typeof(MaybeSingleMaybeArrayConverterFactory))]
    public IEnumerable<string>? Rel { get; set; }
    public string? MediaType { get; set; }
    public string? Name { get; set; }
    public string? Hreflang { get; set; }
    public int? Height { get; set; }
    public int? Width { get; set; }
    public Uri? Preview { get; set; }
}
