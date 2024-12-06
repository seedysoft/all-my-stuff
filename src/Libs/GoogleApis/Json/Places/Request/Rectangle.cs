using Seedysoft.Libs.GoogleApis.Json.Shared;

namespace Seedysoft.Libs.GoogleApis.Json.Places.Request;

public class Rectangle
{
    [J("low")] public required LatitudeLongitude Low { get; set; }
    [J("high")] public required LatitudeLongitude High { get; set; }
}
