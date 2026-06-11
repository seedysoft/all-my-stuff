namespace Seedysoft.Libs.GasStationPrices.ViewModels;

public record class TravelQueryModel
{
    public required Place Orig { get; set; }

    public required Place Dest { get; set; }

    public required int MaxDistanceInKm { get; set; }

    [System.ComponentModel.DataAnnotations.Length(1, int.MaxValue)]
    public IReadOnlyCollection<Constants.ProductoPetroliferoId> PetroleumProductsSelectedIds { get; set; } = [];

    public static TravelQueryModel CreateDefault()
    {
        return new()
        {
            Orig = new Place()
            {
                Address = "CALLE JUAN RAMON JIMENEZ 8, Burgos",
                Latitude = 42.35436,
                Longitude = -3.66279,
            },
            Dest = new Place()
            {
                Address = "CALLE IGLESIA 12, Brazuelo",
                Latitude = 42.49745,
                Longitude = -6.15617,
            },
            MaxDistanceInKm = 10,
            PetroleumProductsSelectedIds = Models.Minetur.ProductoPetrolifero.Gasoline.Select(static x => x.IdProducto).ToHashSet(),
        };
    }
    public static TravelQueryModel CreateEmpty()
    {
        return new()
        {
            Orig = new Place()
            {
                Address = string.Empty,
                Latitude = 0.0,
                Longitude = 0.0,
            },
            Dest = new Place()
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
