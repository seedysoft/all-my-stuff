namespace Seedysoft.Libs.Core.Constants;

public static class Earth
{
    /// <summary>
    /// North Pole: 90d
    /// </summary>
    public const double MaxLatitudeInDegrees = TotalDegrees / 4d;
    /// <summary>
    /// South Pole: -90d
    /// </summary>
    public const double MinLatitudeInDegrees = -MaxLatitudeInDegrees;
    /// <summary>
    /// East from Prime Meridian: 180d
    /// </summary>
    public const double MaxLongitudeInDegrees = TotalDegrees / 2d;
    /// <summary>
    /// West from Prime Meridian: -180d
    /// </summary>
    public const double MinLongitudeInDegrees = -MaxLongitudeInDegrees;

    public const double MeanRadiusInMeters = 6_371_008.8d;

    public const int TotalDegrees = 360;

    public readonly record struct Home
    {
        public const double Lat = 42.354397413084406d;
        public const double Lng = -3.6628374207940815d;

        //"Latitud":  42.364111
        //"Longitud": -3.622139
    }
}
