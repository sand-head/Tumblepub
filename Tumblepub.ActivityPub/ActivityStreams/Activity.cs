using System.Text.Json.Serialization;
using Tumblepub.ActivityPub.Converters;

namespace Tumblepub.ActivityPub.ActivityStreams;

public record Activity(string Type) : Object(Type)
{
    [JsonConverter(typeof(MaybeSingleMaybeArrayConverterFactory))]
    public IEnumerable<ActivityStreamsValue>? Actor { get; init; }
    public Object? Object { get; init; }
    [JsonConverter(typeof(MaybeSingleMaybeArrayConverterFactory))]
    public IEnumerable<ActivityStreamsValue>? Target { get; init; }
}
