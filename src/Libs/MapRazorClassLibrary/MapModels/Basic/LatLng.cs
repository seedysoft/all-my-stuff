namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.Basic;

/// <summary>
/// Represents a geographical point with a certain latitude and longitude.
/// </summary>
/// <param name="lat">Latitude in degrees.</param>
/// <param name="lng">Longitude in degrees.</param>
/// <param name="alt" >Altitude in meters (optional).</param>
[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class LatLng(double lat, double lng, double? alt = default)
{
    /// <summary>
    /// Latitude in degrees.
    /// </summary>
    [J("lat")] public double Lat { get; set; } = lat;

    /// <summary>
    /// Longitude in degrees.
    /// </summary>
    [J("lng")] public double Lng { get; set; } = lng;

    /// <summary>
    /// Altitude in meters (optional).
    /// </summary>
    [J("alt")] public double? Alt { get; set; } = alt;

    /// <summary>
    /// A "fake" id to store a dictionary
    /// </summary>
    [J("key")]
    public string Key =>
        $"{Lat.ToString(Core.Constants.Globalization.NumberFormatInfoInvariant)};{Lng.ToString(Core.Constants.Globalization.NumberFormatInfoInvariant)}";

    internal string GetDebuggerDisplay()
    {
        return Alt.HasValue
            ? $"Lat: {Lat}; Lng: {Lng}; Alt: {Alt}"
            : $"Lat: {Lat}; Lng: {Lng}";
    }
}
