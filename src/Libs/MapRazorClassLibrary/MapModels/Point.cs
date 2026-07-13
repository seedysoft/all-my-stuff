namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

/// <summary>
/// Represents a point with x and y coordinates in pixels.
/// </summary>
public readonly record struct Point
{
    [J("x")] public double X { get; init; }
    [J("y")] public double Y { get; init; }

    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }
}
