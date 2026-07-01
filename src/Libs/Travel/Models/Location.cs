namespace Seedysoft.Libs.Travel.Models;

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
/// <summary>
/// Represents a geographic location using decimal latitude and longitude in degrees.
/// </summary>
/// <param name="Lat">Latitude in decimal degrees. Positive values are north of the equator. Typical range: -90 to 90.</param>
/// <param name="Lon">Longitude in decimal degrees. Positive values are east of the prime meridian. Typical range: -180 to 180.</param>
public readonly record struct Location(decimal Lat, decimal Lon)
{
    public static Location Empty => new(decimal.Zero, decimal.Zero);

    /// <summary>
    /// Produces the string used by the <see cref="System.Diagnostics.DebuggerDisplayAttribute"/>
    /// to show a concise representation in debugger windows (for example: "Lat: 12.34; Lon: 56.78").
    /// </summary>
    /// <returns>A culture-invariant string containing the latitude and longitude.</returns>
    private string GetDebuggerDisplay() =>
        $"Lat: {Lat.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)}; " +
        $"Lon: {Lon.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)}";
}
