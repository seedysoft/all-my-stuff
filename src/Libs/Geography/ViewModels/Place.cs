namespace Seedysoft.Libs.Geography.ViewModels;

public record class Place
{
    public required string Address { get; set; }
    public required float Latitude { get; set; }
    public required float Longitude { get; set; }
}
