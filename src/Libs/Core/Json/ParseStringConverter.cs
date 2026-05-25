using System.Text.Json;

namespace Seedysoft.Libs.Core.Json;

public class ParseStringConverter : System.Text.Json.Serialization.JsonConverter<long>
{
    public override bool CanConvert(Type t) => t == typeof(long);

    public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => long.TryParse(reader.GetString(), out long l) ? l : throw new Exception("Cannot unmarshal type long");

    public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value.ToString(), options);

    public static readonly ParseStringConverter Singleton = new();
}
