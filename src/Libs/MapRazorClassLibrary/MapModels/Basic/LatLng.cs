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
    internal static readonly LatLng Empty = new(0, 0);

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

    internal bool IsEquals(LatLng? args, double? maxDegreesMargin)
    {
        if (args == null)
            return false;

        double margin = Math.Max(Math.Abs(Lat - args.Lat), Math.Abs(Lng - args.Lng));

        return margin <= (maxDegreesMargin ?? 1.0E-9);
    }

    /// <summary>
    /// A "fake" id to store a dictionary
    /// </summary>
    [J("key")]
    public string Key =>
        $"{Lat.ToString(Core.Constants.Globalization.NumberFormatInfoInvariant)};{Lng.ToString(Core.Constants.Globalization.NumberFormatInfoInvariant)}";

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
