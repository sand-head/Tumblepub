using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tumblepub.ActivityPub.Extensions;

namespace Tumblepub.ActivityPub.Converters;

internal class RelativeToAbsoluteUriConverter : JsonConverter<Uri>
{
    private readonly Uri _domain;

    public RelativeToAbsoluteUriConverter(HttpContext context)
    {
        _domain = new Uri($"{context.Request.Scheme}://{context.Request.Host.Value}/");
    }

    public override Uri? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (value == null)
        {
            return null;
        }
        return new Uri(value);
    }

    public override void Write(Utf8JsonWriter writer, Uri value, JsonSerializerOptions options)
    {
        if (!value.IsAbsoluteUri)
        {
            writer.WriteStringValue(value.MakeAbsoluteUri(_domain).ToString());
        }
        else
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
