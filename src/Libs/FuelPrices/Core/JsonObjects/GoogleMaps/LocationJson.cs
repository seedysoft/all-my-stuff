using System.Diagnostics;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Seedysoft.Libs.FuelPrices.Core.JsonObjects.GoogleMaps;

[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public sealed class LocationJson : IEquatable<LocationJson>
{
    public LocationJson() { }
    public LocationJson(double lat, double lng)
    {
        Lat = lat;
        Lng = lng;
    }
    public LocationJson(double? lat, double? lng) : this(lat.GetValueOrDefault(), lng.GetValueOrDefault()) { }

    [JsonPropertyName("lat")]
    public double? Lat { get; set; }
    public double LatNotNull => Lat.GetValueOrDefault();

    [JsonPropertyName("lng")]
    public double? Lng { get; set; }
    public double LngNotNull => Lng.GetValueOrDefault();

    public bool Equals(LocationJson? other) => EqualsPrivate(other);
    public override bool Equals(object? obj) => EqualsPrivate(obj);

    public override int GetHashCode() => base.GetHashCode();

    private bool EqualsPrivate(object? other)
    {
        //Check whether the compared object is null.
        if (other is null)
            return false;

        //Check whether the compared object references the same data.
        if (ReferenceEquals(this, other))
            return true;

        //Check whether the products' properties are equal.
        return Lat.Equals((other as LocationJson)?.Lat) && Lng.Equals((other as LocationJson)?.Lng);
    }

    private string GetDebuggerDisplay() => $"{LatNotNull.ToString(CultureInfo.InvariantCulture)},{LngNotNull.ToString(CultureInfo.InvariantCulture)}";
}
