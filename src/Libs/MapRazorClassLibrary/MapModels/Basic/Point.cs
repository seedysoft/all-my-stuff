namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.Basic;

/// <summary>
/// Represents a point with x and y coordinates in pixels.
/// </summary>
public record class Point
{
    private Point() { }
    public Point(double xy) : this(xy, xy) { }
    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// The x coordinate of the point.
    /// </summary>
    [J("x")] public double X { get; init; }
    /// <summary>
    /// The y coordinate of the point.
    /// </summary>
    [J("y")] public double Y { get; init; }

    public static Point Empty { get; } = new(0, 0);
}
