using System.Text.Json.Serialization;
using Tumblepub.ActivityPub.Converters;

namespace Tumblepub.ActivityPub.ActivityStreams;

public record Activity(string Type) : Object(Type)
{
    [JsonConverter(typeof(MaybeSingleMaybeArrayConverterFactory))]
    public IEnumerable<ActivityStreamsValue>? Actor { get; set; }
    public Object? Object { get; set; }
    [JsonConverter(typeof(MaybeSingleMaybeArrayConverterFactory))]
    public IEnumerable<ActivityStreamsValue>? Target { get; set; }
}
