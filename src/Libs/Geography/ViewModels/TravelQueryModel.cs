namespace Seedysoft.Libs.Geography.ViewModels;

public record class TravelQueryModel
{
    public required Place Origin { get; set; }

    public required Place Destination { get; set; }

    public required int MaxDistanceInKm { get; set; }

    [System.ComponentModel.DataAnnotations.Length(1, int.MaxValue)]
    public IReadOnlyCollection<GasStationPrices.Constants.ProductoPetroliferoId> PetroleumProductsSelectedIds { get; set; } = [];

    public static TravelQueryModel CreateDefault()
    {
        return new()
        {
            Origin = new Place()
            {
                Address = "Calle Juan Ramon Jimenez, 8, Burgos, Spain",
                Latitude = 42.354358f,
                Longitude = -3.662786f,
            },
            Destination = new Place()
            {
                Address = "Calle Iglesia 11, 24715 Brazuelo, León",
                Latitude = 42.541333f,
                Longitude = -6.194499f
            },
            MaxDistanceInKm = 10,
            PetroleumProductsSelectedIds = GasStationPrices.Models.Minetur.ProductoPetrolifero.Gasoline.Select(static x => x.IdProducto).ToHashSet(),
        };
    }
    public static TravelQueryModel CreateEmpty()
    {
        return new()
        {
            Origin = new Place()
            {
                Address = string.Empty,
                Latitude = 0f,
                Longitude = 0f
            },
            Destination = new Place()
            {
                Address = string.Empty,
                Latitude = 0f,
                Longitude = 0f
            },
            MaxDistanceInKm = 10,
            PetroleumProductsSelectedIds = GasStationPrices.Models.Minetur.ProductoPetrolifero.Gasoline.Select(static x => x.IdProducto).ToHashSet(),
        };
    }
}
