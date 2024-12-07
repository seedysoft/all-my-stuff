namespace Seedysoft.Libs.GoogleApis.Models.Shared;

public class Circle
{
    [J("center")]
    public required LatitudeLongitude Center { get; set; }

    [J("radius")]
    public long Radius { get; set; }
}
