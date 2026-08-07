using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent : IAsyncDisposable
{
    private DotNetObjectReference<MapComponent> ObjRef = default!;

    private readonly string MapId = $"map-{Guid.NewGuid()}";

    private bool IsMapReady = false;

    private readonly string LeafletJavascriptFile =
        "_content/Seedysoft.Libs.MapRazorClassLibrary/lib/leaflet/leaflet" +
#if DEBUG
        "-src" +
#endif
       ".js";

    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    [Inject] private GasStationPrices.Services.GasStationPricesService GasStationPricesService { get; set; } = default!;

    [Inject] private Travel.Services.Routing.RoutingService RoutingService { get; set; } = default!;

    private IJSObjectReference? MapModule { get; set; }

    private async Task CreateMapAsync()
    {
        try
        {
            if (MapModule == null)
                return;

            await MapModule.InvokeVoidAsync("createMap", MapId, Map);

            if (OnMapCreatedAsyncEventCallback.HasDelegate)
                await OnMapCreatedAsyncEventCallback.InvokeAsync(this);

            //if (OnMapClickAsync.HasDelegate)
            //    await LeafletService.InvokeVoidAsync("setClickHandler", MapId, ObjRef, nameof(OnMapClick));

            IsMapReady = true;

            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await Console.Out.WriteAsync(ex.Message);
        }
    }

    private async Task AddPolylineAsync(double[,] arrayPolyline, string color)
    {
        if (MapModule != null)
        {
            //await Task.Run(async delegate
            //{
            if (arrayPolyline.GetLength(1) != 2)
                throw new ArgumentException($"The {nameof(arrayPolyline)} must be a 2D array with two columns for latitude and longitude.");

            IEnumerable<MapModels.Basic.LatLng> values =
                from i in Enumerable.Range(0, arrayPolyline.GetLength(0))
                select new MapModels.Basic.LatLng(lat: arrayPolyline[i, 0], lng: arrayPolyline[i, 1]);

            MapModels.VectorLayers.Polyline polyline = new([.. values])
            {
                Color = color,
                Weight = 10,
            };

            await MapModule.InvokeVoidAsync("addPolyline", polyline);
            //});
        }
    }

    //private async Task AddMarkerAsync(
    //    MapModels.UILayers.Marker? marker = default
    //    , MapModels.Basic.Icon? icon = default
    //    , string? popupContent = default) => await MapModule.InvokeVoidAsync("addMarker", marker, icon, popupContent);

    private async Task AddCircleMarker(
        MapModels.VectorLayers.CircleMarker circleMarker,
        OneOf.OneOf<string, MapModels.UILayers.Popup>? popup,
        OneOf.OneOf<string, MapModels.UILayers.Tooltip>? tooltip)
    {
        if (MapModule != null)
        {
            await MapModule.InvokeVoidAsync(
                $"addCircleMarker",
                circleMarker,
                popup?.Value ?? null,
                tooltip?.Value ?? null);
        }
    }

    private async Task LoadProductLimitsAsync(
        GasStationPrices.Models.ProductLimits[] productLimits)
    {
        if (MapModule != null)
        {
            await MapModule.InvokeVoidAsync(
                $"loadProductLimits",
                productLimits);
        }
    }

    private async Task DeleteMapAsync()
    {
        if (MapModule != null)
            await MapModule.InvokeVoidAsync("destroyMap");
    }

    private async Task RemoveAllMarkersAsync()
    {
        if (MapModule != null)
            await MapModule.InvokeVoidAsync("removeAllMarkers");
    }

    // Unused. Uncomment if ncccessary
    //private async Task SetViewAsync(LatLng latLng, int zoom)
    //    => await MapModule.InvokeVoidAsync("setView", latLng, zoom);

    private async Task ShowLoaderAsync()
    {
        if (MapModule != null)
            await MapModule.InvokeVoidAsync("showLoader");
    }

    private async Task HideLoaderAsync()
    {
        if (MapModule != null)
            await MapModule.InvokeVoidAsync("hideLoader");
    }
}
