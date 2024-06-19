namespace Seedysoft.Carburantes.CoreLib.ViewModels;

[System.Diagnostics.DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public sealed class RoutePoint
{
    public NetTopologySuite.Geometries.Geometry PointEpsg3857 { get; set; } = default!;

    private string GetDebuggerDisplay() => PointEpsg3857?.AsText() ?? "NULL";
}
