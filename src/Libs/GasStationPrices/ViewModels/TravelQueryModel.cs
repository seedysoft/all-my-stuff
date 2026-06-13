namespace Seedysoft.Libs.GasStationPrices.ViewModels;

public record class TravelQueryModel
{
    public required Geocoding.ViewModels.Place Orig { get; set; }

    public required Geocoding.ViewModels.Place Dest { get; set; }

    public required int MaxDistanceInKm { get; set; }

    [System.ComponentModel.DataAnnotations.Length(1, int.MaxValue)]
    public IReadOnlyCollection<Constants.ProductoPetroliferoId> PetroleumProductsSelectedIds { get; set; } = [];

    public static TravelQueryModel CreateDefault()
    {
        return new()
        {
            Orig = new Geocoding.ViewModels.Place()
            {
                Address = "CALLE JUAN RAMON JIMENEZ 8, Burgos",
                Latitude = Geocoding.Constants.Earth.Burgos.Lat,
                Longitude = Geocoding.Constants.Earth.Burgos.Lng,
            },
            Dest = new Geocoding.ViewModels.Place()
            {
                Address = "CALLE IGLESIA 12, Brazuelo",
                Latitude = Geocoding.Constants.Earth.Brazuelo.Lat,
                Longitude = Geocoding.Constants.Earth.Brazuelo.Lng,
            },
            MaxDistanceInKm = 10,
            PetroleumProductsSelectedIds = Models.Minetur.ProductoPetrolifero.Gasoline.Select(static x => x.IdProducto).ToHashSet(),
        };
    }
    public static TravelQueryModel CreateEmpty()
    {
        return new()
        {
            Orig = new Geocoding.ViewModels.Place()
            {
                Address = string.Empty,
                Latitude = 0.0,
                Longitude = 0.0,
            },
            Dest = new Geocoding.ViewModels.Place()
            {
                Address = string.Empty,
                Latitude = 0.0,
                Longitude = 0.0,
            },
            MaxDistanceInKm = 10,
            PetroleumProductsSelectedIds = Models.Minetur.ProductoPetrolifero.Gasoline.Select(static x => x.IdProducto).ToHashSet(),
        };
    }
}
