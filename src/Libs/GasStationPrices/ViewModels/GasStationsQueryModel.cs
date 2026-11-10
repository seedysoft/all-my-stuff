using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.GasStationPrices.ViewModels;

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public record GasStationsQueryModel
{
    public required int MaxDistanceInKm { get; set; }

    [System.ComponentModel.DataAnnotations.Length(1, int.MaxValue)]
    public IEnumerable<Constants.ProductoPetroliferoId> PetroleumProductsSelectedIds { get; set; } = [];

    public static GasStationsQueryModel CreateDefault()
    {
        return new()
        {
            MaxDistanceInKm = 10,
            PetroleumProductsSelectedIds = [.. Models.Minetur.ProductoPetrolifero.Gasoline.Select(static x => x.IdProducto)],
        };
    }

    private string GetDebuggerDisplay() => this.ToJson();
}
