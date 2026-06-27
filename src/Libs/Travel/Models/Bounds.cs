namespace Seedysoft.Libs.Travel.Models;

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public record Bounds(Location NorthEast, Location SouthWest)
{
    /// <summary>
    /// The Northest and Eastest point on Earth: <see cref="Location.Northest"/>
    /// </summary>
    public static Location MaxNorthEast { get; } = new(Lat: Limits.North, Lon: Limits.East);
    /// <summary>
    /// The Sout and Westest point on Earth.
    /// </summary>
    public static Location MaxSouthWest { get; } = new(Lat: Limits.South, Lon: Limits.West);

    public static Bounds Empty => new(MaxNorthEast, MaxSouthWest);

    public bool IsInside(Location location)
    {
        return
            location.Lat < NorthEast.Lat &&
            location.Lat > SouthWest.Lat &&
            location.Lon < NorthEast.Lon &&
            location.Lon > SouthWest.Lon;
    }

    private string GetDebuggerDisplay() =>
        $"{nameof(NorthEast)}:{NorthEast};{nameof(SouthWest)}:{SouthWest}";

    public record Limits
    {
        /// <summary>
        /// 90
        /// </summary>
        public const decimal North = 90;
        /// <summary>
        /// -90
        /// </summary>
        public const decimal South = -90;
        /// <summary>
        /// 180
        /// </summary>
        public const decimal East = 180;
        /// <summary>
        /// -180
        /// </summary>
        public const decimal West = -180;
    }
}
