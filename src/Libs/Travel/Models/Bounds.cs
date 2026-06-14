namespace Seedysoft.Libs.Travel.Models;

public class Bounds
{
    public Location NorthEast { get; set; } = new() { Latitude = 0.0, Longitude = 0.0 };
    public Location SouthWest { get; set; } = new() { Latitude = 0.0, Longitude = 0.0 };

    public bool IsInside(Location location)
    {
        return
            location.Latitude < NorthEast.Latitude &&
            location.Latitude > SouthWest.Latitude &&
            location.Longitude < NorthEast.Longitude &&
            location.Longitude > SouthWest.Longitude;
    }
}
