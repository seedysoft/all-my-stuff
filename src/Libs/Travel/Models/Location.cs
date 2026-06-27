namespace Seedysoft.Libs.Travel.Models;

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public record Location(decimal Lat, decimal Lon)
{
    private string GetDebuggerDisplay() =>
        $"Lat:{Lat.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)};" +
        $"Lon:{Lon.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)}";
}
