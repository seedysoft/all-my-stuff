namespace Seedysoft.Libs.Travel.Models;

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
/// <summary>
/// Represents a geographic location using double latitude and longitude in degrees.
/// </summary>
public readonly record struct Location
{
    public Location(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public double Latitude { get; init; }
    public double Longitude { get; init; }

    public static Location Empty => new(0, 0);

    /// <summary>
    /// Produces the string used by the <see cref="System.Diagnostics.DebuggerDisplayAttribute"/>
    /// to show a concise representation in debugger windows (for example: "Lat: 12.34; Lon: 56.78").
    /// </summary>
    /// <returns>A culture-invariant string containing the latitude and longitude.</returns>
    private string GetDebuggerDisplay() =>
        $"Lat: {Latitude.ToString(Core.Constants.Globalization.NumberFormatInfoInvariant)}; " +
        $"Lon: {Longitude.ToString(Core.Constants.Globalization.NumberFormatInfoInvariant)}";

    //    public readonly Location AddMetters(double angle, double distanceInMetters)
    //    {
    //        double latitude = Helpers.CoordinatesCalculatesHelper.GetLatitudeFromDegreesPerMetter(Latitude, angle, distanceInMetters);
    //        double longitude = Helpers.CoordinatesCalculatesHelper.GetLongitudeFromDegreesPerMetter(Latitude, Longitude, angle, distanceInMetters);

    //        return new Location(latitude, longitude);
    //    }

    //    public readonly Location AddKm(double angle, double distanceInKm) =>
    //        AddMetters(angle, distanceInKm * 1_000);

    //    public readonly Location AddCm(double angle, double distanceInKm) =>
    //        AddMetters(angle, distanceInKm / 100);
}
