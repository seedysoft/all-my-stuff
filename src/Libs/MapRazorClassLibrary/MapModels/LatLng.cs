using System.Diagnostics;

namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
//[K(typeof(LatLngJsonConverter))]
public sealed class LatLng(double lat, double lng/*, double? alt = default*/)
{
    [J("lat")] public double Lat { get; set; } = lat;
    [J("lng")] public double Lng { get; set; } = lng;
    //[J("alt")] public double? Alt { get; set; } = alt;

    private string GetDebuggerDisplay()
    {
        //return Alt.HasValue
        //    ? $"Lat: {Lat}; Lng: {Lng}; Alt: {Alt}"
        //    : $"Lat: {Lat}; Lng: {Lng}";
        return $"Lat: {Lat}; Lng: {Lng}";
    }
}

//public class LatLngJsonConverter : System.Text.Json.Serialization.JsonConverter<LatLng>
//{
//    public override LatLng Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
//    {
//        if (reader.TokenType != System.Text.Json.JsonTokenType.StartArray)
//            throw new System.Text.Json.JsonException($"Expected start of array for {nameof(LatLng)}");

//        _ = reader.Read();
//        if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
//            throw new System.Text.Json.JsonException("Expected latitude number");

//        double latitude = reader.GetDouble();

//        _ = reader.Read();
//        if (reader.TokenType != System.Text.Json.JsonTokenType.Number)
//            throw new System.Text.Json.JsonException("Expected longitude number");

//        double longitude = reader.GetDouble();

//        _ = reader.Read();
//#pragma warning disable IDE0046 // Convert to conditional expression
//        if (reader.TokenType != System.Text.Json.JsonTokenType.EndArray)
//            throw new System.Text.Json.JsonException($"Expected end of array for {nameof(LatLng)}");

//        return new LatLng(latitude, longitude);
//#pragma warning restore IDE0046 // Convert to conditional expression
//    }

//    public override void Write(System.Text.Json.Utf8JsonWriter writer, LatLng value, System.Text.Json.JsonSerializerOptions options)
//    {
//        writer.WriteStartArray();
//        writer.WriteNumberValue(value.Lat);
//        writer.WriteNumberValue(value.Lng);
//        writer.WriteEndArray();
//    }
//}
