namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.Basic;

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class LatLngBounds(LatLng northEast, LatLng southWest)
{
    [J("_northEast")] public LatLng NorthEast { get; init; } = northEast;
    [J("_southWest")] public LatLng SouthWest { get; init; } = southWest;

    public static Travel.Models.Bounds Copy(LatLngBounds latLngBounds) =>
        new(new Travel.Models.Location(latLngBounds.NorthEast.Lat, latLngBounds.NorthEast.Lng),
            new Travel.Models.Location(latLngBounds.SouthWest.Lat, latLngBounds.SouthWest.Lng));

    private string GetDebuggerDisplay() =>
        NorthEast.GetDebuggerDisplay() + " " + SouthWest.GetDebuggerDisplay();
}
