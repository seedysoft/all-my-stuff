using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.Travel.ViewModels;

/// <summary>
/// Represents a geographical location with an address and coordinates.
/// </summary>
/// <remarks>
/// This record includes a debugger display that shows the JSON representation of the place.
/// </remarks>
/// <param name="Address">The street address of the location.</param>
/// <param name="Location">The coordinates for the place as a <see cref="Models.Location"/> (latitude and longitude).</param>
[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly record struct Place(string Address, Models.Location Location)
{
    public static Place Empty => new(string.Empty, Models.Location.Empty);

    /// <summary>
    /// Gets the JSON representation of this place for debugger display purposes.
    /// </summary>
    /// <returns>A JSON string representation of the place.</returns>
    private string GetDebuggerDisplay() => this.ToJson();
}
