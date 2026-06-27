using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.Travel.ViewModels;

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public record Place(string Address, decimal Lat, decimal Lon)
{
    [I()] public Models.Location Location => new(Lat, Lon);

    private string GetDebuggerDisplay() => this.ToJson();
}
