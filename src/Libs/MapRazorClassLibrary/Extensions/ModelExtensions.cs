namespace Seedysoft.Libs.MapRazorClassLibrary.Extensions;

public static class ModelExtensions
{
    public static Travel.Models.Bounds ToBounds(this MapModels.Basic.LatLngBounds latLngBounds) =>
        new(new Core.Models.Location(latLngBounds.NorthEast.Lat, latLngBounds.NorthEast.Lng),
            new Core.Models.Location(latLngBounds.SouthWest.Lat, latLngBounds.SouthWest.Lng));

    public static Core.Models.Location ToLocation(this MapModels.Basic.LatLng latLng) => new(latLng.Lat, latLng.Lng);
}
