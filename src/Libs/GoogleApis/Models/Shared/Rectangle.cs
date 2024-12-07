namespace Seedysoft.Libs.GoogleApis.Models.Shared;

public class Rectangle
{
    [J("low")]
    public required LatitudeLongitude Low { get; set; }

    [J("high")]
    public required LatitudeLongitude High { get; set; }
}
