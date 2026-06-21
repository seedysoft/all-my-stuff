namespace Seedysoft.Libs.Travel.Models;

public class Bounds
{
    public Location NorthEast { get; set; } = new(lat: decimal.Zero, lon: decimal.Zero);
    public Location SouthWest { get; set; } = new(lat: decimal.Zero, lon: decimal.Zero);

    public bool IsInside(Location location)
    {
        return
            location.Lat < NorthEast.Lat &&
            location.Lat > SouthWest.Lat &&
            location.Lon < NorthEast.Lon &&
            location.Lon > SouthWest.Lon;
    }
}
