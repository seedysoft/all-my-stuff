namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.Basic;

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class LatLngBounds(LatLng northEast, LatLng southWest)
{
    internal static readonly LatLngBounds Empty = new(LatLng.Empty, LatLng.Empty);

    [J("_northEast")] public LatLng NorthEast { get; init; } = northEast;
    [J("_southWest")] public LatLng SouthWest { get; init; } = southWest;

    internal bool IsEquals(LatLngBounds? args, double? maxDegreesMargin) =>
        SouthWest.IsEquals(args?.SouthWest, maxDegreesMargin) &&
        NorthEast.IsEquals(args?.NorthEast, maxDegreesMargin);

    private string GetDebuggerDisplay() =>
        NorthEast.GetDebuggerDisplay() + " " + SouthWest.GetDebuggerDisplay();
}
