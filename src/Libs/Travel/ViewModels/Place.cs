using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.Travel.ViewModels;

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public record class Place
{
    public required string Address { get; set; }
    public required decimal Lat { get; set; }
    public required decimal Lon { get; set; }

    [I()] public Models.Location Location => new(lat: Lat, lon: Lon);

    private string GetDebuggerDisplay() => this.ToJson();
}
