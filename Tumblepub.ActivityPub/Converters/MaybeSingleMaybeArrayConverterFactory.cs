using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tumblepub.ActivityPub.Converters;

internal class MaybeSingleMaybeArrayConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType || typeToConvert.GetGenericTypeDefinition() != typeof(IEnumerable<>))
        {
            return false;
        }

        return true;
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var valueType = typeToConvert.GetGenericArguments()[0];

        return (JsonConverter)Activator.CreateInstance(
            typeof(MaybeSingleMaybeArrayConverter<>).MakeGenericType(new Type[] { valueType }),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: new object[] { options },
            culture: null)!;
    }

    private class MaybeSingleMaybeArrayConverter<TValue> : JsonConverter<IEnumerable<TValue>>
    {
        public override IEnumerable<TValue>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                return JsonSerializer.Deserialize<IEnumerable<TValue>>(ref reader);
            }

            var value = JsonSerializer.Deserialize<TValue>(ref reader, options);
            if (value == null)
            {
                return null;
            }

            return new List<TValue>
            {
                value
            };
        }

        public override void Write(Utf8JsonWriter writer, IEnumerable<TValue> value, JsonSerializerOptions options)
        {
            if (!value.Any())
            {
                writer.WriteNullValue();
            }
            else if (value.Count() == 1)
            {
                JsonSerializer.Serialize(writer, value.First(), options);
            }
            else
            {
                JsonSerializer.Serialize(writer, value, options);
            }
        }
    }
}
