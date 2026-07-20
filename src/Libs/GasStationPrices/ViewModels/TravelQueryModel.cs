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
                "Calle Juan Ramón Jiménez, 8, Burgos, Castilla y León, España",
                Travel.Constants.Earth.Burgos
            ),
            Dest = new Travel.ViewModels.Place(
                "Calle de la Iglesia, Brazuelo, Castilla y León, España",
                Travel.Constants.Earth.Brazuelo
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
            Orig = Travel.ViewModels.Place.Empty,
            Dest = Travel.ViewModels.Place.Empty,
            MaxDistanceInKm = 10,
            PetroleumProductsSelectedIds = [.. Models.Minetur.ProductoPetrolifero.Gasoline.Select(static x => x.IdProducto)],
        };
    }
#endif

    private string GetDebuggerDisplay() => this.ToJson();
}
