using System.Reflection;
using System.Text.Json;

namespace GoogleMapsLibrary.Serialization;

/// <summary>
/// 
/// </summary>
public class EnumMemberConverter<T> : JsonConverter<T> where T : IComparable, IFormattable, IConvertible
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? jsonValue = reader.GetString();

        foreach (FieldInfo fieldInfo in typeToConvert.GetFields())
        {
            var description = fieldInfo.GetCustomAttribute(typeof(EnumMemberAttribute), false) as EnumMemberAttribute;

            if (string.Equals(jsonValue, description?.Value, StringComparison.OrdinalIgnoreCase))
                return (T?)fieldInfo.GetValue(default);
        }

        throw new JsonException($"string {jsonValue} was not found as a description in the enum {typeToConvert}");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        FieldInfo? fi = value.GetType().GetField($"{value}");

        var description = fi?.GetCustomAttribute(typeof(EnumMemberAttribute), false) as EnumMemberAttribute;

        writer.WriteStringValue(description?.Value);
    }
}
