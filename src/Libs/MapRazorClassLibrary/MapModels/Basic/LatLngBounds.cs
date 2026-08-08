namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.Basic;

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class LatLngBounds(LatLng corner1, LatLng corner2)
{
    private readonly LatLng Corner1 = corner1;
    private readonly LatLng Corner2 = corner2;

    public static Travel.Models.Bounds Copy(LatLngBounds latLngBounds) =>
        new(new Travel.Models.Location(latLngBounds.Corner1.Lat, latLngBounds.Corner1.Lng),
            new Travel.Models.Location(latLngBounds.Corner2.Lat, latLngBounds.Corner2.Lng));

    private string GetDebuggerDisplay() =>
        Corner1.GetDebuggerDisplay() + " " + Corner2.GetDebuggerDisplay();
}
