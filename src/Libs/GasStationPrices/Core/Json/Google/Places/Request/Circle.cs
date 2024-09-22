namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Places.Request;

public class Circle
{
    [J("center")] public required Shared.LatitudeLongitude Center { get; set; }
    [J("radius")] public long Radius { get; set; }
}
