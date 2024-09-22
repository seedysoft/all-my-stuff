namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Shared;

public class LatitudeLongitude()
{
    [J("latitude")] public double Latitude { get; set; }
    [J("longitude")] public double Longitude { get; set; }

    public string ToQueryString() => $"{Latitude}+{Longitude}";
}
