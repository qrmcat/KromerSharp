using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kromer.Utils;

public class NullAsFalseConverter<T> : JsonConverter<T?> where T : class
{
    public override bool HandleNull => true;

    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType == JsonTokenType.False 
            ? null 
            : JsonSerializer.Deserialize<T>(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteBooleanValue(false);
        }
        else
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}

public class NullAsFalseConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) => !typeToConvert.IsValueType;

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = typeof(NullAsFalseConverter<>).MakeGenericType(typeToConvert);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}