using System.Text.Json;
using System.Text.Json.Serialization;
using Tumblepub.ActivityPub.ActivityStreams;

namespace Tumblepub.ActivityPub.Converters;

internal class LinkConverter : JsonConverter<Link>
{
    public override Link? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var link = reader.GetString()!;
            return new Link(new Uri(link));
        }

        return JsonSerializer.Deserialize<Link>(ref reader);
    }

    public override void Write(Utf8JsonWriter writer, Link value, JsonSerializerOptions options)
    {
        // check all properties to see if any other than Href (and of course Context and Type) are not null
        var nullCondition = value.GetType().GetProperties()
            .Where(p => p.Name != nameof(Link.Href) && p.Name != nameof(Link.Context) && p.Name != nameof(Link.Type))
            .Any(p => p.GetValue(value) != null);

        if (!nullCondition)
        {
            // just write Href as a string
            JsonSerializer.Serialize(writer, value.Href, options);
        }
        else
        {
            // serialize the whole thing
            JsonSerializer.Serialize(writer, value);
        }
    }
}
