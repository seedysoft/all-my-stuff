using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.GasStationPrices.ViewModels;

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public record TravelQueryModel
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
            Orig = new Travel.ViewModels.Place(
                Address: "Calle Juan Ramón Jiménez, 8, Burgos, Castilla y León, España",
                Lat: Travel.Constants.Earth.Burgos.Lat,
                Lon: Travel.Constants.Earth.Burgos.Lon
            ),
            Dest = new Travel.ViewModels.Place(
                Address: "Calle de la Iglesia, Brazuelo, Castilla y León, España",
                Lat: Travel.Constants.Earth.Brazuelo.Lat,
                Lon: Travel.Constants.Earth.Brazuelo.Lon
            ),
            MaxDistanceInKm = 10,
            PetroleumProductsSelectedIds = [.. Models.Minetur.ProductoPetrolifero.Gasoline.Select(static x => x.IdProducto)],
        };
    }
#else
    public static TravelQueryModel CreateEmpty()
    {
        return new()
        {
            Orig = new Travel.ViewModels.Place(string.Empty, decimal.Zero, decimal.Zero),
            Dest = new Travel.ViewModels.Place(string.Empty, decimal.Zero, decimal.Zero),
            MaxDistanceInKm = 10,
            PetroleumProductsSelectedIds = Models.Minetur.ProductoPetrolifero.Gasoline.Select(static x => x.IdProducto).ToHashSet(),
        };
    }
#endif

    private string GetDebuggerDisplay() => this.ToJson();
}
