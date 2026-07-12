namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class LatLng(double lat, double lng, double? alt = default)
{
    [J("lat")] public double Lat { get; set; } = lat;
    [J("lng")] public double Lng { get; set; } = lng;
    [J("alt")] public double? Alt { get; set; } = alt;

    private string GetDebuggerDisplay()
    {
        return Alt.HasValue
            ? $"Lat: {Lat}; Lng: {Lng}; Alt: {Alt}"
            : $"Lat: {Lat}; Lng: {Lng}";
    }
}
