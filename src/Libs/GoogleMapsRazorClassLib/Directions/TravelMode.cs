using System.Text.Json;
using System.Text.Json.Serialization;

namespace Seedysoft.Libs.GoogleMapsRazorClassLib.Directions;

/// <summary>
/// The valid travel modes that can be specified in a DirectionsRequest as well as the travel modes returned in a DirectionsStep. 
/// </summary>
[JsonConverter(typeof(TravelModeJsonConverter))]
public enum TravelMode
{
    /// <summary>
    /// Specifies a bicycling directions request.
    /// </summary>
    Bicycling,

    /// <summary>
    /// Specifies a driving directions request.
    /// </summary>
    Driving,

    /// <summary>
    /// Specifies a transit directions request.
    /// </summary>
    Transit,

    /// <summary>
    /// Specifies a walking directions request.
    /// </summary>
    Walking,
}

public class TravelModeJsonConverter : JsonConverter<TravelMode>
{
    public override bool CanConvert(Type t) => t == typeof(TravelMode);

    public override TravelMode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? value = reader.GetString();

        return string.IsNullOrWhiteSpace(value)
            ? default
            : (TravelMode)Enum.Parse(typeof(TravelMode), value[..1].ToUpperInvariant() + value[1..].ToLowerInvariant());
    }

    public override void Write(Utf8JsonWriter writer, TravelMode value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value.ToString().ToUpperInvariant(), options);

    public static readonly TravelModeJsonConverter Singleton = new();
}
