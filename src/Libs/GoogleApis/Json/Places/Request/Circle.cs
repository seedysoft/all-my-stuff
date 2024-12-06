using Seedysoft.Libs.GoogleApis.Json.Shared;

namespace Seedysoft.Libs.GoogleApis.Json.Places.Request;

public class Circle
{
    [J("center")] public required LatitudeLongitude Center { get; set; }
    [J("radius")] public long Radius { get; set; }
}
