namespace Seedysoft.Libs.GasStationPrices.Constants;

public static class Earth
{
    /// <summary>
    /// 
    /// </summary>
    public readonly record struct Home
    {
        /// <summary>
        /// Gets the longitude of the home location.
        /// </summary>
        private const double Lng = -3.6628374207940815d;
        /// <summary>
        /// Gets the latitude of the home location.
        /// </summary>
        private const double Lat = 42.354397413084406d;

        public static (double Lng, double Lat) Center { get; } = new(Lng, Lat);
    }
}
