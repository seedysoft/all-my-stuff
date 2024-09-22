namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Places.Request;

public class Rectangle
{
    [J("low")] public required Shared.LatitudeLongitude Low { get; set; }
    [J("high")] public required Shared.LatitudeLongitude High { get; set; }
}
