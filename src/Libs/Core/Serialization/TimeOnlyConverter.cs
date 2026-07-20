//namespace Seedysoft.Libs.Core.Serialization;

//public class TimeOnlyConverter(string? serializationFormat) : System.Text.Json.Serialization.JsonConverter<TimeOnly>
//{
//    private readonly string serializationFormat = serializationFormat ?? "HH:mm:ss.fff";

//    public TimeOnlyConverter() : this(null) { }

//    public override TimeOnly Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
//        => TimeOnly.Parse(reader.GetString()!);

//    public override void Write(System.Text.Json.Utf8JsonWriter writer, TimeOnly value, System.Text.Json.JsonSerializerOptions options)
//        => writer.WriteStringValue(value.ToString(serializationFormat));
//}
