using System.Text.Json.Serialization;
using Tumblepub.ActivityPub.Converters;

namespace Tumblepub.ActivityPub.ActivityStreams;

public abstract record ActivityStreamsValue(string Type)
{
    [JsonPropertyName("@context"), JsonConverter(typeof(MaybeSingleMaybeArrayConverterFactory))]
    public IEnumerable<string> Context { get; set; } = new List<string>();
}
