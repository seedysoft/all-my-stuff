using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.Core.Serialization;

public class EnumMemberJsonConverter<T> : System.Text.Json.Serialization.JsonConverter<T> where T : Enum
{
    public override bool CanConvert(Type t) => t == typeof(T);

    public override T? Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        => EnumExtensions.ToEnum<T>(reader.GetString()!);

    public override void Write(System.Text.Json.Utf8JsonWriter writer, T value, System.Text.Json.JsonSerializerOptions options)
        => System.Text.Json.JsonSerializer.Serialize(writer, value.GetEnumMember(), options);

    //public static readonly EnumMemberJsonConverter<T> Singleton = new();
}
