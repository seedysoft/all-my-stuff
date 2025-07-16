namespace Seedysoft.Libs.Core.Constants;

public static class Earth
{
    /// <summary>
    /// North Pole: 90d
    /// </summary>
    public const int MaxLatitudeInDegrees = TotalDegrees / 4;
    /// <summary>
    /// South Pole: -90d
    /// </summary>
    public const int MinLatitudeInDegrees = -MaxLatitudeInDegrees;
    /// <summary>
    /// East from Prime Meridian: 180d
    /// </summary>
    public const int MaxLongitudeInDegrees = TotalDegrees / 2;
    /// <summary>
    /// West from Prime Meridian: -180d
    /// </summary>
    public const int MinLongitudeInDegrees = -MaxLongitudeInDegrees;

    private const double MeanRadiusInMeters = 6_371_008.8d;
    public const double MeanRadiusInKilometers = MeanRadiusInMeters / 1_000d;

    public const int TotalDegrees = 360;

    public readonly record struct Home
    {
        public const double Lat = 42.354397413084406d;
        public const double Lng = -3.6628374207940815d;

        //"Latitud":  42.364111
        //"Longitud": -3.622139
    }
}
