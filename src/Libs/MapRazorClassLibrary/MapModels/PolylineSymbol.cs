namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

internal class PolylineSymbol
{
    [J("color")] public string? Color { get; set; } = "black";

    [J("weight")] public int Weight { get; set; } = 1;

    [J("opacity")] public double Opacity { get; set; } = 1.0;

    [J("smoothFactor")] public double SmoothFactor { get; set; } = 1.0;
}
