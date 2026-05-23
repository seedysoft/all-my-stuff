using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Seedysoft.Libs.Core.Extensions;
using Seedysoft.Libs.MapRazorClassLib.Models;

namespace Seedysoft.Libs.MapRazorClassLib;

/// <summary>
/// A Blazor component that renders an interactive map with customizable dimensions and zoom level.
/// </summary>
/// <remarks>
/// This component integrates with JavaScript interop to initialize and manage a map instance.
/// The map element is identified by its <see cref="Id"/> parameter and styled based on the provided 
/// CSS parameters and style attributes.
/// </remarks
public partial class MapComponent : ComponentBase, IAsyncDisposable
{
    /// <summary>
    /// Gets or sets the unique identifier for the map component element.
    /// </summary>
    /// <remarks>
    /// This parameter is required and must be provided when using the component.
    /// The Id is used to identify the DOM element and communicate with the JavaScript map initialization function.
    /// </remarks>
    [EditorRequired]
    [Parameter] public required string Id { get; set; }

    /// <summary>
    /// Gets or sets the center position of the map using latitude and longitude coordinates.
    /// </summary>
    /// <remarks>
    /// Default value is <see langword="default" />.
    /// </remarks>
    [Parameter] public LngLat Center { get; set; } = default!;

    /// <summary>
    /// Gets or sets additional CSS class names to apply to the map component container.
    /// </summary>
    /// <remarks>
    /// These classes are combined with the internally generated classes from dimension parameters.
    /// Default value is <see langword="null" />.
    /// </remarks>
    [Parameter] public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the height of the <see cref="MapComponent"/>.
    /// </summary>
    /// <remarks>
    /// Default value is <see langword="null" />.
    /// The actual height applied depends on both this value and the <see cref="HeightUnit"/> parameter.
    /// </remarks>
    [Parameter] public double? Height { get; set; }

    /// <summary>
    /// Gets or sets the units for the <see cref="Height"/> dimension.
    /// </summary>
    /// <remarks>
    /// Default value is <see cref="Core.Enums.CssUnit.Px"/>.
    /// Valid units include pixels (Px), percentages (Percentage), and other CSS unit types.
    /// </remarks>
    [Parameter] public Core.Enums.CssUnit HeightUnit { get; set; } = Core.Enums.CssUnit.Px;

    /// <summary>
    /// Gets or sets inline CSS styles to apply directly to the map component container.
    /// </summary>
    /// <remarks>
    /// These styles are merged with the internally generated styles from dimension parameters.
    /// Default value is <see langword="null" />.
    /// </remarks>
    [Parameter] public string? Style { get; set; }

    /// <summary>
    /// Gets or sets the width of the <see cref="MapComponent"/>.
    /// </summary>
    /// <remarks>
    /// Default value is <see langword="null" />.
    /// The actual width applied depends on both this value and the <see cref="WidthUnit"/> parameter.
    /// </remarks>
    [Parameter] public double? Width { get; set; }

    /// <summary>
    /// Gets or sets the units for the <see cref="Width"/> dimension.
    /// </summary>
    /// <remarks>
    /// Default value is <see cref="Core.Enums.CssUnit.Percentage"/>.
    /// Valid units include pixels (Px), percentages (Percentage), and other CSS unit types.
    /// </remarks>
    [Parameter] public Core.Enums.CssUnit WidthUnit { get; set; } = Core.Enums.CssUnit.Percentage;

    /// <summary>
    /// Gets or sets the zoom level of the <see cref="MapComponent"/>.
    /// </summary>
    /// <remarks>
    /// Default value is 11.
    /// Higher values increase the zoom level, while lower values decrease it.
    /// </remarks>
    [Parameter] public double Zoom { get; set; } = 11.0;

    /// <summary>
    /// Gets a reference to the underlying DOM element for the map component.
    /// </summary>
    /// <remarks>
    /// This reference can be used for direct DOM manipulation if needed through JavaScript interop.
    /// </remarks>
    public ElementReference Element { get; set; }

    /// <summary>
    /// Gets the combined inline CSS classes for the map component, including m-container if not already present.
    /// </summary>
    /// <remarks>
    /// This property builds the class string based on the provided Class parameter.
    /// </remarks>
    protected string? ClassNames =>
        BuildClassNames(
            Class,
            ("m-container", !Class?.Contains("m-container") ?? true)
        );
    /// <summary>
    /// Gets the combined inline CSS styles for the map component, including width, height, and custom styles.
    /// </summary>
    /// <remarks>
    /// This property builds the style string based on the provided Width, Height, and Style parameters.
    /// Only styles with valid dimensions greater than zero are included.
    /// </remarks>
    protected string? StyleNames =>
        BuildStyleNames(
            Style,
            ($"height:{Height!.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}{HeightUnit.ToCssString()}", Height.GetValueOrDefault() > 0),
            ($"width:{Width!.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}{WidthUnit.ToCssString()}", Width.GetValueOrDefault() > 0)
        );

    /// <summary>
    /// Gets or sets the JavaScript runtime instance used for JavaScript interop calls.
    /// </summary>
    [Inject] protected IJSRuntime JsRuntime { get; set; } = default!;

    private IJSObjectReference? _module;

    /// <summary>
    /// Executes after the component has rendered, initializing the map via JavaScript interop.
    /// </summary>
    /// <param name="firstRender"><see langword="true"/> if this is the first render of the component; otherwise <see langword="false"/>.</param>
    /// <remarks>
    /// This method loads the map JavaScript module and invokes the createMap function to initialize the map instance.
    /// The module is cached to avoid reloading it on subsequent renders.
    /// </remarks>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            _module ??= await JsRuntime.InvokeAsync<IJSObjectReference>("import", $"./_content/Seedysoft.Libs.MapRazorClassLib/map.js");

            await _module.InvokeVoidAsync($"createMap", Id, Center, Zoom);
        }
    }

    /// <summary>
    /// Builds a combined CSS class string from a user-defined class and a collection of conditional CSS classes.
    /// </summary>
    /// <param name="userDefinedCssClass">Optional user-defined CSS classes to include.</param>
    /// <param name="cssClassList">A collection of tuples containing CSS class names and conditions determining whether they should be included.</param>
    /// <returns>A space-separated string of CSS class names, or an empty string if no classes are included.</returns>
    /// <remarks>
    /// Only classes with a true condition and non-empty class names are included in the result.
    /// The user-defined class is always included if provided, regardless of any conditions.
    /// </remarks>
    private static string BuildClassNames(string? userDefinedCssClass, params (string? cssClass, bool when)[] cssClassList)
    {
        var list = cssClassList
            .Where(static x => x.when && !string.IsNullOrWhiteSpace(x.cssClass))
            .Select(static x => x.cssClass)
            .ToHashSet();

        if (!string.IsNullOrWhiteSpace(userDefinedCssClass))
            _ = list.Add(userDefinedCssClass.Trim());

        return list.Count == 0 ? string.Empty : string.Join(" ", list);
    }
    /// <summary>
    /// Builds a combined CSS style string from user-defined styles and a collection of conditional CSS styles.
    /// </summary>
    /// <param name="userDefinedCssStyle">Optional user-defined inline CSS styles to include.</param>
    /// <param name="cssStyleList">A collection of tuples containing CSS style declarations and conditions determining whether they should be included.</param>
    /// <returns>A semicolon-separated string of CSS style declarations, or an empty string if no styles are included.</returns>
    /// <remarks>
    /// Only styles with a true condition and non-empty style declarations are included in the result.
    /// The user-defined style is always included if provided, regardless of any conditions.
    /// Each style should be a complete declaration (e.g., "width:100px").
    /// </remarks>
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

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        return _module?.DisposeAsync() ?? default;
    }
}
