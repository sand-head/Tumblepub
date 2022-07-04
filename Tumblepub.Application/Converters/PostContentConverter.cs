using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tumblepub.Application.Aggregates;

namespace Tumblepub.Application.Converters;

public class PostContentConverter : JsonConverter<PostContent>
{
    public override PostContent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);

        return document.RootElement.GetProperty("$type").GetString() switch
        {
            "internal" => document.Deserialize<PostContent.Internal>(options),
            "external" => document.Deserialize<PostContent.External>(options),
            "shared" => document.Deserialize<PostContent.Shared>(options),
            "deleted" => document.Deserialize<PostContent.Deleted>(options),
            _ => throw new JsonException("Unknown PostContent type"),
        };
    }

    public override void Write(Utf8JsonWriter writer, PostContent value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        writer.WriteString("$type", GetType(value));

        foreach (var property in value.GetType().GetProperties())
        {
            if (!property.CanRead || property.GetCustomAttribute<JsonIgnoreAttribute>() != null)
                continue;
            var propertyValue = property.GetValue(value);
            writer.WritePropertyName(property.Name);
            JsonSerializer.Serialize(writer, propertyValue, property.PropertyType, options);
        }
        
        writer.WriteEndObject();
    }
    
    private static string GetType(PostContent value)
    {
        return value switch
        {
            PostContent.Internal => "internal",
            PostContent.External => "external",
            PostContent.Shared => "shared",
            PostContent.Deleted => "deleted",
            _ => throw new JsonException("Unknown PostContent type")
        };
    }
}