namespace Seedysoft.Libs.Core.Serialization;

public class ParseStringJsonConverter : System.Text.Json.Serialization.JsonConverter<long>
{
    public override bool CanConvert(Type t) => t == typeof(long);

    public override long Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        => long.TryParse(reader.GetString(), out long l) ? l : throw new Exception("Cannot unmarshal type long");

    public override void Write(System.Text.Json.Utf8JsonWriter writer, long value, System.Text.Json.JsonSerializerOptions options)
        => System.Text.Json.JsonSerializer.Serialize(writer, value.ToString(), options);

    //public static readonly ParseStringJsonConverter Singleton = new();
}
