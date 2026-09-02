using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.Core.Serialization;

public class EnumMemberJsonConverter<TEnum> : System.Text.Json.Serialization.JsonConverter<TEnum> where TEnum : Enum
{
    public override bool CanConvert(Type t) => t == typeof(TEnum);

    public override TEnum? Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        => EnumExtensions.ToEnum<TEnum>(reader.GetString()!);

    public override void Write(System.Text.Json.Utf8JsonWriter writer, TEnum value, System.Text.Json.JsonSerializerOptions options)
        => System.Text.Json.JsonSerializer.Serialize(writer, value.GetEnumMember(), options);

    //public static readonly EnumMemberJsonConverter<T> Singleton = new();
}
