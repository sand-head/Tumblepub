using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tumblepub.ActivityPub.ActivityStreams;
using Object = Tumblepub.ActivityPub.ActivityStreams.Object;

namespace Tumblepub.ActivityPub.Converters;

internal class ActivityStreamsValueConverter : JsonConverter<ActivityStreamsValue>
{
    public override ActivityStreamsValue? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        using var jsonDocument = JsonDocument.ParseValue(ref reader);
        if (!jsonDocument.RootElement.TryGetProperty(nameof(ActivityStreamsValue.Type), out var typeProperty))
        {
            throw new JsonException();
        }

        Type type;
        var activityStreamsType = typeProperty.GetString();

        if (Activity.Types.Contains(activityStreamsType))
        {
            type = typeof(Activity);
        }
        else if (Actor.Types.Contains(activityStreamsType))
        {
            type = typeof(Actor);
        }
        else if (Collection.Types.Contains(activityStreamsType))
        {
            type = typeof(Collection);
        }
        else if (Link.Types.Contains(activityStreamsType))
        {
            type = typeof(Link);
        }
        else if (Object.Types.Contains(activityStreamsType))
        {
            type = typeof(Object);
        }
        else if (OrderedCollection.Types.Contains(activityStreamsType))
        {
            type = typeof(OrderedCollection);
        }
        else if (activityStreamsType == "OrderedCollectionPage")
        {
            type = typeof(OrderedCollection<>);
        }
        else
        {
            throw new JsonException();
        }

        var jsonObject = jsonDocument.RootElement.GetRawText();
        return (ActivityStreamsValue?)JsonSerializer.Deserialize(jsonObject, type, options);
    }

    public override void Write(Utf8JsonWriter writer, ActivityStreamsValue value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, options);
    }
}
