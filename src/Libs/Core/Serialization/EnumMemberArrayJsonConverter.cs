//using Seedysoft.Libs.Core.Extensions;

//namespace Seedysoft.Libs.Core.Serialization;

//public class EnumMemberArrayJsonConverter<T> : System.Text.Json.Serialization.JsonConverter<T[]> where T : Enum
//{
//    public override bool CanConvert(Type t) => t == typeof(T[]);

//    public override T[]? Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
//        => [.. reader.GetString()!.Split(',')!.Select(static x => EnumExtensions.ToEnum<T>(x)!)];

//    public override void Write(System.Text.Json.Utf8JsonWriter writer, T[] value, System.Text.Json.JsonSerializerOptions options)
//        => System.Text.Json.JsonSerializer.Serialize(writer, string.Join(",", value.Select(static x => x.GetEnumMember())), options);

//    public static readonly EnumMemberArrayJsonConverter<T> Singleton = new();
//}
