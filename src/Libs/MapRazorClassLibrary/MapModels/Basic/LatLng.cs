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
    public double Lat { get; set; } = Math.Round(lat, 6);

    /// <summary>
    /// Longitude in degrees.
    /// </summary>
    public double Lng { get; set; } = Math.Round(lng, 6);

    /// <summary>
    /// Altitude in meters (optional).
    /// </summary>
    public double? Alt { get; set; } = alt.HasValue ? Math.Round(alt.Value, 6) : null;

    internal string GetDebuggerDisplay()
    {
        return Alt.HasValue
            ? $"Lat: {Lat}; Lng: {Lng}; Alt: {Alt}"
            : $"Lat: {Lat}; Lng: {Lng}";
    }
}
