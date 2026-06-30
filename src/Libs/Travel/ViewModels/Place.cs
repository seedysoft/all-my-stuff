using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.Travel.ViewModels;

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public record Place(string Address, decimal Lat, decimal Lon)
{
    public Place(string Address, Models.Location Location) : this(Address, Location.Lat, Location.Lon) { }

    [I()] public Models.Location Location => new(Lat, Lon);

    private string GetDebuggerDisplay() => this.ToJson();
}
