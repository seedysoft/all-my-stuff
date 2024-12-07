using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Seedysoft.Libs.Core.Enums;
using Seedysoft.Libs.Utils.Extensions;
using System.Globalization;

namespace Seedysoft.Libs.GoogleMapsRazorClassLib.GoogleMap;

public partial class Map : SeedysoftComponentBase
{
    private DotNetObjectReference<Map>? objRef;

    private readonly List<Marker> Markers = [];

    private string GoogleMapsJsFileUrl
        => $"https://maps.googleapis.com/maps/api/js?key={ApiKey}&loading=async&v=beta&libraries=maps,marker,routes";

    protected override string? StyleNames =>
        BuildStyleNames(
            Style,
            ($"width:{Width!.Value.ToString(CultureInfo.InvariantCulture)}{WidthUnit.ToCssString()}", Width.GetValueOrDefault() > 0),
            ($"height:{Height!.Value.ToString(CultureInfo.InvariantCulture)}{HeightUnit.ToCssString()}", Height.GetValueOrDefault() > 0)
        );

    protected override async Task OnInitializedAsync()
    {
        objRef ??= DotNetObjectReference.Create(this);

        await base.OnInitializedAsync();
    }

    /// <summary>
    /// Adds a marker to the GoogleMap.
    /// </summary>
    /// <param name="marker">The marker to add to the map.</param>
    /// <returns>A completed task.</returns>
    public async ValueTask AddMarkerAsync(Marker marker)
    {
        await JSRuntime.InvokeVoidAsync($"{Constants.SeedysoftGoogleMaps}.addMarker", Id, marker, objRef);
        Markers.Add(marker);
    }
    public async ValueTask RemoveAllMarkersAsync()
    {
        await JSRuntime.InvokeVoidAsync($"{Constants.SeedysoftGoogleMaps}.removeAllMarkers", Id, objRef);
        Markers.Clear();
    }

    [JSInvokable]
    public async Task OnMarkerClickJS(Marker marker)
    {
        if (OnMarkerClick.HasDelegate)
            await OnMarkerClick.InvokeAsync(marker);
    }

    private void OnScriptLoad()
    {
        _ = Task.Run(async () => await JSRuntime.InvokeVoidAsync(
            $"{Constants.SeedysoftGoogleMaps}.initialize",
            Id,
            Zoom,
            Center,
            Markers,
            IsClickable,
            objRef));
    }
    private static void OnScriptError(string errorMessage) => throw new Exception(errorMessage);

    /// <summary>
    /// Gets or sets the Google Map API key.
    /// </summary>
    [EditorRequired]
    [Parameter] public required string ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the center parameter.
    /// </summary>
    [Parameter] public GoogleApis.Models.Shared.LatLngLiteral Center { get; set; } = default!;

    /// <summary>
    /// Gets or sets the height of the <see cref="GoogleMap" />.
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
    [Parameter] public CssUnit HeightUnit { get; set; } = CssUnit.Px;

    /// <summary>
    /// Event fired when a user clicks on a marker.
    /// This event fires only when <see cref="Clickable" /> is set to <see langword="true" />.
    /// </summary>
    [Parameter] public EventCallback<Marker> OnMarkerClick { get; set; }

    /// <summary>
    /// Makes the marker clickable if set to <see langword="true" />.
    /// </summary>
    [Parameter] public bool IsClickable { get; set; }

    /// <summary>
    /// Gets or sets the width of the <see cref="GoogleMap" />.
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
    [Parameter] public CssUnit WidthUnit { get; set; } = CssUnit.Percentage;

    /// <summary>
    /// Gets or sets the zoom level of the <see cref="GoogleMap" />.
    /// </summary>
    /// <remarks>
    /// Default value is 14.
    /// </remarks>
    [Parameter] public int Zoom { get; set; } = 14;
}
