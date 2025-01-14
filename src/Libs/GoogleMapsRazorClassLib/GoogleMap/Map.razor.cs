using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RestSharp;
using Seedysoft.Libs.Core.Extensions;
using System.Collections.Frozen;

namespace Seedysoft.Libs.GoogleMapsRazorClassLib.GoogleMap;

public partial class Map : SeedysoftComponentBase
{
    /// <summary>
    /// Gets or sets the Google Map API key.
    /// </summary>
    [EditorRequired, Parameter] public required string ApiKey { get; set; }

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
    [Parameter] public Core.Enums.CssUnit HeightUnit { get; set; } = Core.Enums.CssUnit.Px;

    /// <summary>
    /// Event fired when a user clicks on a marker.
    /// This event fires only when <see cref="Clickable" /> is set to <see langword="true" />.
    /// </summary>
    [Parameter] public EventCallback<Marker> OnClickGoogleMapMarker { get; set; }

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
    [Parameter] public Core.Enums.CssUnit WidthUnit { get; set; } = Core.Enums.CssUnit.Percentage;

    /// <summary>
    /// Gets or sets the zoom level of the <see cref="GoogleMap" />.
    /// </summary>
    /// <remarks>
    /// Default value is 14.
    /// </remarks>
    [Parameter] public int Zoom { get; set; } = 14;

    private DotNetObjectReference<Map>? objRef;

    private readonly HashSet<Marker> markers = [];
    //public System.Collections.ObjectModel.ReadOnlyCollection<Marker> Markers => markers.AsReadOnly();

    private string GoogleMapsJsFileUrl
        => $"https://maps.googleapis.com/maps/api/js?key={ApiKey}&loading=async&v=beta&libraries=maps,marker,routes";

    protected override string? StyleNames =>
        BuildStyleNames(
            Style,
            ($"width:{Width!.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}{WidthUnit.ToCssString()}", Width.GetValueOrDefault() > 0),
            ($"height:{Height!.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}{HeightUnit.ToCssString()}", Height.GetValueOrDefault() > 0)
        );

    protected override async Task OnInitializedAsync()
    {
        objRef ??= DotNetObjectReference.Create(this);

        await base.OnInitializedAsync();
    }

    private async ValueTask AddMarkerAsync(Marker marker)
    {
        await JSRuntime.InvokeVoidAsync($"{Constants.SeedysoftGoogleMaps}.addMarker", Id, marker, objRef);
        Markers.Add(marker);
    }
    public async ValueTask RemoveAllMarkersAsync()
    {
        await JSRuntime.InvokeVoidAsync($"{Constants.SeedysoftGoogleMaps}.removeAllMarkers", Id, objRef);
        Markers.Clear();
    }
    public async ValueTask ClickOnMarkerAsync(Marker marker)
    {
        if (!markers.Any(x => x.Id == marker.Id))
            await AddMarkerAsync(marker);

        await JSRuntime.InvokeVoidAsync($"{Constants.SeedysoftGoogleMaps}.openInfoWindow", Id, marker, objRef);
    }

    public async Task<IReadOnlySet<GoogleApis.Models.Shared.LatLngLiteral>> HighlightRouteAsync(int routeIndex)
    {
        var DirLeg = DotNetObjectReference.Create<string>(string.Empty);
        var objects = await JSRuntime.InvokeAsync<GoogleApis.Models.Directions.Response.DirectionsLeg>(
            $"{Constants.SeedysoftGoogleMaps}.highlightRoute",
#if DEBUG
                TimeSpan.FromSeconds(15),
#else
                TimeSpan.FromSeconds(2),
#endif
            [Id, routeIndex, DirLeg]);

        return FrozenSet<GoogleApis.Models.Shared.LatLngLiteral>.Empty;
    }

    [JSInvokable]
    public async Task OnClickGoogleMapMarkerJS(Marker marker)
    {
        if (OnClickGoogleMapMarker.HasDelegate)
            await OnClickGoogleMapMarker.InvokeAsync(marker);
    }

    private static void OnScriptError(string errorMessage) => throw new Exception(errorMessage);
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

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        try
        {
            objRef?.Dispose();
        }
        catch (JSDisconnectedException) { }
    }
    protected override async ValueTask DisposeAsyncCore(bool disposing)
    {
        await base.DisposeAsyncCore(disposing);
        try
        {
            objRef?.Dispose();
        }
        catch (JSDisconnectedException) { }
    }
}
