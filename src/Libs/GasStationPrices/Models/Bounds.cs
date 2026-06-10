namespace Seedysoft.Libs.GasStationPrices.Models;

public class Bounds
{
    public Location NorthEast { get; set; } = new Location
    {
        Latitude = 0.0,
        Longitude = 0.0
    };
    public Location SouthWest { get; set; } = new Location
    {
        Latitude = 0.0,
        Longitude = 0.0
    };

    public bool IsInside(Minetur.EstacionTerrestre e)
    {
        return
            e.Lat < NorthEast.Latitude &&
            e.Lat > SouthWest.Latitude &&
            e.Lng < NorthEast.Longitude &&
            e.Lng > SouthWest.Longitude;
    }
}
