namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Places.Request;

public class Location
{
    [J("circle")] public Circle? Circle { get; set; }
    [J("rectangle")] public Rectangle? Rectangle { get; set; }
}
