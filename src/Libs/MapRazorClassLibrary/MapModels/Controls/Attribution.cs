namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.Controls;

/// <summary>
/// The attribution control allows you to display attribution data in a small text box on a map.
/// It is put on the map by default unless you set its <see cref="Map.AttributionControl"/> option to <c>false</c>, and it fetches attribution texts from layers with the getAttribution method automatically. 
/// Extends <see cref="Base.Control"/>.
/// </summary>
public sealed record class Attribution : Base.Control
{
    public Attribution() : base() => Position = Positions.BottomRight;

    /// <summary>
    /// The HTML text shown before the attributions.
    /// Pass <c>false</c> to disable.
    /// </summary>
    /// <remarks>Default: 'Leaflet'</remarks>
    [J("prefix")] public OneOf.OneOf<bool, string>? Prefix { get; set; } = "Leaflet";
}
