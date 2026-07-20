namespace Seedysoft.Libs.Travel.Models;

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public record Bounds(Location NorthEast, Location SouthWest)
{
    /// <summary>
    /// The Northest and Eastest point on Earth: <see cref="Location.Northest"/>
    /// </summary>
    public static Location MaxNorthEast { get; } = new(latitude: Limits.North, longitude: Limits.East);
    /// <summary>
    /// The Sout and Westest point on Earth.
    /// </summary>
    public static Location MaxSouthWest { get; } = new(latitude: Limits.South, longitude: Limits.West);

    public static Bounds Empty => new(NorthEast: MaxNorthEast, SouthWest: MaxSouthWest);
    public static Bounds Inverse => new(NorthEast: MaxSouthWest, SouthWest: MaxNorthEast);

    public bool IsInside(Location location)
    {
        return
            location.Latitude < NorthEast.Latitude &&
            location.Latitude > SouthWest.Latitude &&
            location.Longitude < NorthEast.Longitude &&
            location.Longitude > SouthWest.Longitude;
    }

    private string GetDebuggerDisplay() =>
        $"{nameof(NorthEast)}:{NorthEast};{nameof(SouthWest)}:{SouthWest}";

    private record Limits
    {
        /// <summary>
        /// 90
        /// </summary>
        public const double North = 90;
        /// <summary>
        /// -90
        /// </summary>
        public const double South = -90;
        /// <summary>
        /// 180
        /// </summary>
        public const double East = 180;
        /// <summary>
        /// -180
        /// </summary>
        public const double West = -180;
    }
}
