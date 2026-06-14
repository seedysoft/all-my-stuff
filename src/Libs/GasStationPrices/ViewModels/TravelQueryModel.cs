namespace Seedysoft.Libs.GasStationPrices.ViewModels;

public record class TravelQueryModel
{
    public required Travel.ViewModels.Place Orig { get; set; }

    public required Travel.ViewModels.Place Dest { get; set; }

    public required int MaxDistanceInKm { get; set; }

    [System.ComponentModel.DataAnnotations.Length(1, int.MaxValue)]
    public IReadOnlyCollection<Constants.ProductoPetroliferoId> PetroleumProductsSelectedIds { get; set; } = [];

    public static TravelQueryModel CreateDefault()
    {
        return new()
        {
            Orig = new Travel.ViewModels.Place()
            {
                Address = "CALLE JUAN RAMON JIMENEZ 8, Burgos",
                Latitude = Travel.Constants.Earth.Burgos.Lat,
                Longitude = Travel.Constants.Earth.Burgos.Lng,
            },
            Dest = new Travel.ViewModels.Place()
            {
                Address = "CALLE IGLESIA 12, Brazuelo",
                Latitude = Travel.Constants.Earth.Brazuelo.Lat,
                Longitude = Travel.Constants.Earth.Brazuelo.Lng,
            },
            MaxDistanceInKm = 10,
            PetroleumProductsSelectedIds = Models.Minetur.ProductoPetrolifero.Gasoline.Select(static x => x.IdProducto).ToHashSet(),
        };
    }
    //public static TravelQueryModel CreateEmpty()
    //{
    //    return new()
    //    {
    //        Orig = new Travel.ViewModels.Place()
    //        {
    //            Address = string.Empty,
    //            Latitude = 0.0,
    //            Longitude = 0.0,
    //        },
    //        Dest = new Travel.ViewModels.Place()
    //        {
    //            Address = string.Empty,
    //            Latitude = 0.0,
    //            Longitude = 0.0,
    //        },
    //        MaxDistanceInKm = 10,
    //        PetroleumProductsSelectedIds = Models.Minetur.ProductoPetrolifero.Gasoline.Select(static x => x.IdProducto).ToHashSet(),
    //    };
    //}
}
