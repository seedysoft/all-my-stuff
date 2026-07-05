namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

internal class SimpleDirection
{
    [J("polyline")] public string? Polyline { get; set; }

    [J("start")] public int? Start { get; set; }

    [J("radianAngle")] public double? RadianAngle { get; set; }

    [J("length")] public double? Length { get; set; }

    [J("symbol")] public PolylineSymbol? Symbol { get; set; }
}
