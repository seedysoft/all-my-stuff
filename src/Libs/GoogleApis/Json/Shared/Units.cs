using System.Text.Json;
using System.Text.Json.Serialization;

namespace Seedysoft.Libs.GoogleApis.Json.Shared;

/// <summary>
/// The valid unit systems that can be specified in a DirectionsRequest.
/// </summary>
[K(typeof(UnitsJsonConverter))]
public enum Units
{
    /// <summary>
    /// Specifies that distances in the DirectionsResult should be expressed in imperial units.
    /// </summary>
    Imperial,

    /// <summary>
    /// Specifies that distances in the DirectionsResult should be expressed in metric units.
    /// </summary>
    Metric,
}

public class UnitsJsonConverter : JsonConverter<Units>
{
    public override bool CanConvert(Type t) => t == typeof(Units);

    public override Units Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? value = reader.GetString();

        return string.IsNullOrWhiteSpace(value)
            ? default
            : (Units)Enum.Parse(typeof(Units), value[..1].ToUpperInvariant() + value[1..].ToLowerInvariant());
    }

    public override void Write(Utf8JsonWriter writer, Units value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, $"{value.ToString().ToUpperInvariant()}", options);
    public static readonly UnitsJsonConverter Singleton = new();
}
