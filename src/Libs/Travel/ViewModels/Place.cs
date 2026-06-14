namespace Seedysoft.Libs.Travel.ViewModels;

public record class Place
{
    public required string Address { get; set; }
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }

    public Models.Location Location => new() { Latitude = Latitude, Longitude = Longitude, };
}
