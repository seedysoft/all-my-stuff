namespace Seedysoft.Libs.Travel.Models;

public class Location(decimal lat, decimal lon)
{
    public decimal Lat { get; set; } = lat;
    public decimal Lon { get; set; } = lon;
}
