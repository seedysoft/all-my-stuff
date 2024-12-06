namespace Seedysoft.Libs.GoogleApis.Json.Places.Request;

public class Location
{
    [J("circle")] public Circle? Circle { get; set; }
    [J("rectangle")] public Rectangle? Rectangle { get; set; }
}
