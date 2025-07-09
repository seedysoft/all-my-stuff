using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RestSharp;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.GoogleMapsRazorClassLib.GoogleMap;

public partial class Map : SeedysoftComponentBase
{
    #region Parameters

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
    [Parameter] public Core.Enums.CssUnit HeightUnit { get; set; } = Core.Enums.CssUnit.Px;

    ///// <summary>
    ///// Event fired when a user clicks on a marker.
    ///// This event fires only when <see cref="Clickable" /> is set to <see langword="true" />.
    ///// </summary>
    //[Parameter] public EventCallback<Marker> OnClickGmapMarkerEventCallback { get; set; }
    /// <summary>
    /// Event fired when a user clicks on a route.
    /// </summary>
    [Parameter] public EventCallback<string> OnClickGmapRouteEventCallback { get; set; }

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

    #endregion

    private DotNetObjectReference<Map>? objRef;

    private readonly HashSet<Marker> markers = [];
    //public System.Collections.ObjectModel.ReadOnlyCollection<Marker> Markers => markers.AsReadOnly();

    //  => $"https://maps.googleapis.com/maps/api/js?key=AIzaSyAOWd855Jru-vGD_bVJqc6Qr-n8VpX0XsA&v=beta&libraries=marker,geometry,places&callback=initMap
    private string GoogleMapsJsFileUrl
        => $"https://maps.googleapis.com/maps/api/js?key={ApiKey}&loading=async&v=beta&libraries=maps,geometry,marker,places,routes";

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

    private async Task AddGasStationMarkerAsync(Marker marker)
    {
        await JSRuntime.InvokeVoidAsync($"{Constants.SeedysoftGoogleMaps}.addGasStationMarker", Id, marker, objRef);
        _ = markers.Add(marker);
    }
    public async Task RemoveAllMarkersAsync()
    {
        await JSRuntime.InvokeVoidAsync($"{Constants.SeedysoftGoogleMaps}.removeAllMarkers", Id, objRef);
        markers.Clear();
    }
    public async Task ClickOnMarkerAsync(Marker marker)
    {
        if (!markers.Any(x => x.Id == marker.Id))
            await AddGasStationMarkerAsync(marker);

        //await JSRuntime.InvokeVoidAsync($"{Constants.SeedysoftGoogleMaps}.openInfoWindow", Id, marker, objRef);
    }

    public async Task SearchRoutesAsync(string origin, string destination)
        => await JSRuntime.InvokeVoidAsync($"{Constants.SeedysoftGoogleMaps}.searchRoutes", TimeSpan.FromSeconds(5), [Id, origin, destination, ApiKey]);

    public async Task ResetViewportAsync() 
        => await JSRuntime.InvokeVoidAsync($"{Constants.SeedysoftGoogleMaps}.resetViewport", Id);

    //[JSInvokable]
    //public async Task OnClickGmapMarkerJS(Marker marker)
    //{
    //    if (OnClickGmapMarkerEventCallback.HasDelegate)
    //        await OnClickGmapMarkerEventCallback.InvokeAsync(marker);
    //}
    [JSInvokable]
    public async Task OnClickGmapRouteJS(string encodedPolyline)
    {
        if (OnClickGmapRouteEventCallback.HasDelegate)
            await OnClickGmapRouteEventCallback.InvokeAsync(encodedPolyline);
    }

    private static void OnScriptError(string errorMessage) => throw new Exception(errorMessage);
    private void OnScriptLoad()
    {
        _ = Task.Run(async () => await JSRuntime.InvokeVoidAsync(
            $"{Constants.SeedysoftGoogleMaps}.init",
            Id,
            Zoom,
            Center,
            IsClickable,
            objRef));
    }

    //protected override void Dispose(bool disposing)
    //{
    //    base.Dispose(disposing);
    //    try
    //    {
    //        objRef?.Dispose();
    //    }
    //    catch (JSDisconnectedException) { }
    //}
    //protected override async ValueTask DisposeAsyncCore(bool disposing)
    //{
    //    await base.DisposeAsyncCore(disposing);
    //    try
    //    {
    //        objRef?.Dispose();
    //    }
    //    catch (JSDisconnectedException) { }
    //}
}
