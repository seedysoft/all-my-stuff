namespace Seedysoft.Libs.MapRazorClassLibrary.Extensions;

public static class ModelExtensions
{
    public static Travel.Models.Location ToLocation(this MapModels.Basic.LatLng latLng) => new(latLng.Lat, latLng.Lng);
}
