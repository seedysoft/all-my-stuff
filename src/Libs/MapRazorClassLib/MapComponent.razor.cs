using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.MapRazorClassLib;

public partial class MapComponent : ComponentBase
{
    [EditorRequired]
    [Parameter] public required string Id { get; set; }

    /// <summary>
    /// Gets or sets the center parameter.
    /// </summary>
    [Parameter] public LatLng Center { get; set; } = default!;

    /// <summary>
    /// Gets or sets the height of the <see cref="MapComponent" />.
    /// </summary>
    /// <remarks>
    /// Default value is <see langword="null" />.
    /// </remarks>
    [Parameter] public double? Height { get; set; }
    /// <summary>
    /// Gets or sets the units for the <see cref="Height" />.
    /// </summary>
    /// <remarks>
    /// Default value is <see cref="CssUnit.Px" />.
    /// </remarks>
    [Parameter] public Seedysoft.Libs.Core.Enums.CssUnit HeightUnit { get; set; } = Seedysoft.Libs.Core.Enums.CssUnit.Px;

    [Parameter] public string? Style { get; set; }

    /// <summary>
    /// Gets or sets the width of the <see cref="MapComponent" />.
    /// </summary>
    /// <remarks>
    /// Default value is <see langword="null" />.
    /// </remarks>
    [Parameter] public double? Width { get; set; }
    /// <summary>
    /// Gets or sets the units for the <see cref="Width" />.
    /// </summary>
    /// <remarks>
    /// Default value is <see cref="CssUnit.Percentage" />.
    /// </remarks>
    [Parameter] public Core.Enums.CssUnit WidthUnit { get; set; } = Core.Enums.CssUnit.Percentage;

    /// <summary>
    /// Gets or sets the zoom level of the <see cref="MapComponent" />.
    /// </summary>
    /// <remarks>
    /// Default value is 14.
    /// </remarks>
    [Parameter] public int Zoom { get; set; } = 14;

    public ElementReference Element { get; set; }

    protected string? StyleNames =>
        BuildStyleNames(
            Style,
            ($"width:{Width!.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}{WidthUnit.ToCssString()}", Width.GetValueOrDefault() > 0),
            ($"height:{Height!.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}{HeightUnit.ToCssString()}", Height.GetValueOrDefault() > 0)
        );

    [Inject] protected IJSRuntime JsRuntime { get; set; } = default!;

    private IJSObjectReference? _module;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        _module ??= await JsRuntime.InvokeAsync<IJSObjectReference>("import", $"./_content/Seedysoft.Libs.MapRazorClassLib/map.js");

        await _module.InvokeVoidAsync($"createMap", Id, null);
    }

    private static string BuildStyleNames(string? userDefinedCssStyle, params (string? cssStyle, bool when)[] cssStyleList)
    {
        var list = cssStyleList
            .Where(static x => x.when && !string.IsNullOrWhiteSpace(x.cssStyle))
            .Select(static x => x.cssStyle)
            .ToHashSet();

        if (!string.IsNullOrWhiteSpace(userDefinedCssStyle))
            _ = list.Add(userDefinedCssStyle.Trim());

        return list.Count == 0 ? string.Empty : string.Join(';', list);
    }
}
