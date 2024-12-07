namespace Seedysoft.Libs.GoogleApis.Models.Shared;

public class Location
{
    [J("circle")]
    public Circle? Circle { get; set; }

    [J("rectangle")]
    public Rectangle? Rectangle { get; set; }
}
