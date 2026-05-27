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

    /// <summary>
    /// 
    /// </summary>
    public readonly record struct Home
    {
        // TODO USAR LA PROYECCIÓN ADECUADA Y LOS PUNTOS X E Y CORRESPONDIENTES

        //public const double Lat = 42.354397413084406d;
        //public const double Lng = -3.6628374207940815d;
        public const double Lat = 4687870.56d;
        public const double Lng = 442692.88d;

        // -3.6957,  42.3410,   854.22,EPSG:4258
        // 442692.88,4687870.56,854.22,EPSG:25830

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static LngLat ToLngLat() => new(Lng, Lat);

        //"Latitud":  42.364111
        //"Longitud": -3.622139
    }
}
