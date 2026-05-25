using Seedysoft.Libs.Core.Extensions;
using System.Text.Json;

namespace Seedysoft.Libs.Core.Json;

public class EnumMemberArrayJsonConverter<T> : System.Text.Json.Serialization.JsonConverter<T[]> where T : Enum
{
    public override bool CanConvert(Type t) => t == typeof(T[]);

    public override T[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => [.. reader.GetString()!.Split(',')!.Select(static x => EnumExtensions.ToEnum<T>(x)!)];

    public override void Write(Utf8JsonWriter writer, T[] value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, string.Join(",", value.Select(static x => x.GetEnumMember())), options);
}
