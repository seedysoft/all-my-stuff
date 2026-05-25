using Seedysoft.Libs.Core.Constants;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Seedysoft.Libs.Core.Json;

public class LngLatJsonConverter() : JsonConverter<LngLat>
{
    public override LngLat? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("Expected start of array.");

        _ = reader.Read(); // move to first number
        double lng = reader.GetDouble();

        _ = reader.Read(); // move to second number
        double lat = reader.GetDouble();

        _ = reader.Read(); // move past end of array

        return new LngLat( lng, lat);
    }

    public override void Write(Utf8JsonWriter writer, LngLat value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(value.Lng);
        writer.WriteNumberValue(value.Lat);
        writer.WriteEndArray();
    }
}
