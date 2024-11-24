//using System.Text.Json;
//using System.Text.Json.Serialization;

//namespace Seedysoft.Libs.GoogleMapsRazorClassLib.Directions;

///// <summary>
///// The valid unit systems that can be specified in a DirectionsRequest.
///// </summary>
//[JsonConverter(typeof(UnitSystemJsonConverter))]
//public enum UnitSystem
//{
//    /// <summary>
//    /// Specifies that distances in the DirectionsResult should be expressed in imperial units.
//    /// </summary>
//    Imperial,

//    /// <summary>
//    /// Specifies that distances in the DirectionsResult should be expressed in metric units.
//    /// </summary>
//    Metric,
//}

//public class UnitSystemJsonConverter : JsonConverter<UnitSystem>
//{
//    public override bool CanConvert(Type t) => t == typeof(UnitSystem);

//    public override UnitSystem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//    {
//        string? value = reader.GetString();

//        return string.IsNullOrWhiteSpace(value)
//            ? default
//            : (UnitSystem)Enum.Parse(typeof(UnitSystem), value[..1].ToUpperInvariant() + value[1..].ToLowerInvariant());
//    }

//    public override void Write(Utf8JsonWriter writer, UnitSystem value, JsonSerializerOptions options)
//        => JsonSerializer.Serialize(writer, $"{value.ToString().ToUpperInvariant()}", options);
//    //google.maps.UnitSystem.
//    public static readonly UnitSystemJsonConverter Singleton = new();
//}
