//using System.Text.Json;

//namespace Seedysoft.Libs.Core.Serialization;

//public class DateOnlyConverter(string? serializationFormat = "yyyy-MM-dd") : System.Text.Json.Serialization.JsonConverter<DateOnly>
//{
//    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//        => DateOnly.Parse(reader.GetString()!);

//    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
//        => writer.WriteStringValue(value.ToString(serializationFormat));

//    //public static readonly DateOnlyConverter Singleton = new();
//}
