using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.Travel.ViewModels;

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public record TravelQueryModel
{
    public required Place Orig { get; set; }

    public required Place Dest { get; set; }

#if DEBUG
    public static TravelQueryModel CreateDefault()
    {
        return new()
        {
            Orig = new Place(
                Address: "Calle Juan Ramón Jiménez, 8, Burgos, Castilla y León, España",
                Location: Constants.Earth.Burgos
            ),
            Dest = new Place(
                Address: "Calle de la Iglesia, Brazuelo, Castilla y León, España",
                Location: Constants.Earth.Brazuelo
            ),
        };
    }
#else
    public static TravelQueryModel CreateEmpty()
    {
        return new()
        {
            Orig = Place.Empty,
            Dest = Place.Empty,
        };
    }
#endif

    private string GetDebuggerDisplay() => this.ToJson();
}
