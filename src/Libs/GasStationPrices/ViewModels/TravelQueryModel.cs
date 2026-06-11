namespace Seedysoft.Libs.GasStationPrices.ViewModels;

public record class TravelQueryModel
{
    public required Place Origin { get; set; }

    public required Place Destination { get; set; }

    public required int MaxDistanceInKm { get; set; }

    [System.ComponentModel.DataAnnotations.Length(1, int.MaxValue)]
    public IReadOnlyCollection<Constants.ProductoPetroliferoId> PetroleumProductsSelectedIds { get; set; } = [];

    public static TravelQueryModel CreateDefault()
    {
        return new()
        {
            Origin = new Place()
            {
                Address = "CALLE JUAN RAMON JIMENEZ 8, Burgos",
                Latitude = 42.35436f,
                Longitude = -3.66278553f,
            },
            Destination = new Place()
            {
                Address = "CALLE IGLESIA 12, Brazuelo",
                Latitude = 42.497448f,
                Longitude = -6.1561656f
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
