using System.Text.Json;

namespace GoogleMapsLibrary.Serialization;

/// <summary>
/// TODO apply to all enum with EnumMember attribute
/// </summary>
/// <typeparam name="TEnum"></typeparam>
public class JsonStringEnumConverterEx<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    private readonly Dictionary<TEnum, string> _enumToString = [];
    private readonly Dictionary<string, TEnum> _stringToEnum = [];

    public JsonStringEnumConverterEx()
    {
        Type type = typeof(TEnum);
        Array values = Enum.GetValues(typeof(TEnum));

        foreach (object? value in values)
        {
            System.Reflection.MemberInfo enumMember = type.GetMember(value.ToString() ?? string.Empty)[0];
            EnumMemberAttribute? attr = enumMember
                .GetCustomAttributes(typeof(EnumMemberAttribute), false)
                .Cast<EnumMemberAttribute>()
                .FirstOrDefault();

            _stringToEnum.Add(value.ToString() ?? string.Empty, (TEnum)value);

            if (attr?.Value != null)
            {
                _enumToString.Add((TEnum)value, attr.Value);
                _stringToEnum.Add(attr.Value, (TEnum)value);
            }
            else
            {
                _enumToString.Add((TEnum)value, value.ToString() ?? string.Empty);
            }
        }
    }

    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? stringValue = reader.GetString();

        return _stringToEnum.GetValueOrDefault(stringValue ?? string.Empty);
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options) => writer.WriteStringValue(_enumToString[value]);

    public static string ToLowerFirstChar(string str)
    {
        return string.IsNullOrEmpty(str) || char.IsLower(str, 0)
            ? str
            : char.ToLowerInvariant(str[0]) + str[1..];
    }
}
