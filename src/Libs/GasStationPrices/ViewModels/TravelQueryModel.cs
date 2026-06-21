using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.GasStationPrices.ViewModels;

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public record class TravelQueryModel
{
    public required Travel.ViewModels.Place Orig { get; set; }

    public required Travel.ViewModels.Place Dest { get; set; }

    public required int MaxDistanceInKm { get; set; }

    [System.ComponentModel.DataAnnotations.Length(1, int.MaxValue)]
    public IReadOnlyCollection<Constants.ProductoPetroliferoId> PetroleumProductsSelectedIds { get; set; } = [];

#if DEBUG
    public static TravelQueryModel CreateDefault()
    {
        return new()
        {
            Orig = new Travel.ViewModels.Place()
            {
                Address = "8, Calle Juan Ramón Jiménez, Barrio Juan Pablo II - G9, Barriada de la Inmaculada, Gamonal, Distrito Este, Burgos, Castilla y León, 09007, España",
                Lat = Travel.Constants.Earth.Burgos.Lat,
                Lon = Travel.Constants.Earth.Burgos.Lon,
            },
            Dest = new Travel.ViewModels.Place()
            {
                Address = "Calle de la Iglesia, Brazuelo, León, Castilla y León, 24715, España",
                Lat = Travel.Constants.Earth.Brazuelo.Lat,
                Lon = Travel.Constants.Earth.Brazuelo.Lon,
            },
            MaxDistanceInKm = 10,
            PetroleumProductsSelectedIds = Models.Minetur.ProductoPetrolifero.Gasoline.Select(static x => x.IdProducto).ToHashSet(),
        };
    }
#else
    public static TravelQueryModel CreateEmpty()
    {
        return new()
        {
            Orig = new Travel.ViewModels.Place()
            {
                Address = string.Empty,
                Lat = decimal.Zero,
                Lon = decimal.Zero,
            },
            Dest = new Travel.ViewModels.Place()
            {
                Address = string.Empty,
                Lat = decimal.Zero,
                Lon = decimal.Zero,
            },
            MaxDistanceInKm = 10,
            PetroleumProductsSelectedIds = Models.Minetur.ProductoPetrolifero.Gasoline.Select(static x => x.IdProducto).ToHashSet(),
        };
    }
#endif

    private string GetDebuggerDisplay() => this.ToJson();
}
