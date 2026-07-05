namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

[K(typeof(PointJsonConverter))]
public class Point
{
    public double Latitude { get; }
    public double Longitude { get; }
    
    public Point() { }
    [System.Text.Json.Serialization.JsonConstructor]
    public Point(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
}

public class PointJsonConverter : System.Text.Json.Serialization.JsonConverter<Point>
{
    public override Point Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
    {
        if (reader.TokenType != System.Text.Json.JsonTokenType.StartArray)
            throw new System.Text.Json.JsonException($"Expected start of array for {nameof(Point)}");

        _ = reader.Read();
        if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
            throw new System.Text.Json.JsonException("Expected latitude number");

        double latitude = reader.GetDouble();

        _ = reader.Read();
        if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
            throw new System.Text.Json.JsonException("Expected longitude number");

        double longitude = reader.GetDouble();

        _ = reader.Read();
#pragma warning disable IDE0046 // Convert to conditional expression
        if (reader.TokenType != System.Text.Json.JsonTokenType.EndArray)
            throw new System.Text.Json.JsonException($"Expected end of array for {nameof(Point)}");

        return new Point(latitude, longitude);
#pragma warning restore IDE0046 // Convert to conditional expression
    }

    public override void Write(System.Text.Json.Utf8JsonWriter writer, Point value, System.Text.Json.JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(value.Latitude);
        writer.WriteNumberValue(value.Longitude);
        writer.WriteEndArray();
    }
}
