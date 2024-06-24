namespace Seedysoft.BlazorWebApp.Client.ViewModels;

[System.Diagnostics.DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public sealed class TravelPoint
{
    public NetTopologySuite.Geometries.Geometry PointEpsg3857 { get; set; } = default!;

    private string GetDebuggerDisplay() => PointEpsg3857?.AsText() ?? "NULL";
}
