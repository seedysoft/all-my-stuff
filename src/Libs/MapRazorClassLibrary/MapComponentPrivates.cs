using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent : IAsyncDisposable
{
    private DotNetObjectReference<MapComponent> ObjRef = default!;

    private readonly string MapId = $"map-{Guid.NewGuid()}";

    private bool IsMapReady = false;

    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    [Inject] private GasStationPrices.Services.GasStationPricesService GasStationPricesService { get; set; } = default!;

    [Inject] private Travel.Services.Routing.RoutingService RoutingService { get; set; } = default!;

    private async Task CreateMapAsync()
    {
        try
        {
            await MapModule.InvokeVoidAsync("createMap", MapId, Options);

            if (OnMapCreatedAsyncEventCallback.HasDelegate)
                await OnMapCreatedAsyncEventCallback.InvokeAsync(this);

            //if (OnMapClickAsync.HasDelegate)
            //    await LeafletService.InvokeVoidAsync("setClickHandler", MapId, ObjRef, nameof(OnMapClick));
        }
        catch (Exception ex)
        {
            await Console.Out.WriteAsync(ex.Message);
        }

        IsMapReady = true;

        await InvokeAsync(StateHasChanged);
    }

    private async Task AddPolylineAsync(double[,] arrayPolyline, string color)
    {
        await Task.Run(async delegate
        {
            if (arrayPolyline.GetLength(1) != 2)
                throw new ArgumentException("The arrayPolyline must be a 2D array with two columns for latitude and longitude.");

            IEnumerable<MapModels.LatLng> values =
            from i in Enumerable.Range(0, arrayPolyline.GetLength(0))
            select new MapModels.LatLng(lat: arrayPolyline[i, 0], lng: arrayPolyline[i, 1]);

            MapModels.Polyline polyline = new([.. values], new MapModels.PolylineOptions()
            {
                Color = color,
                Weight = 10,
            });

            await MapModule.InvokeVoidAsync("addPolyline", MapId, polyline);
        });
    }

    //private async Task EnableSpinnerAsync() =>
    //    await LeafletService.InvokeVoidAsync("enableSpinner", MapId);
    //private async Task DisableSpinnerAsync() =>
    //    await LeafletService.InvokeVoidAsync("disableSpinner", MapId);

    //private string GetIcon(Icon icon)
    //{
    //    string useIcon = icon switch
    //    {
    //        Icon.DRON => "drone",
    //        Icon.HOME => "home",
    //        Icon.DESTINATION => "destination",
    //        _ => "marker-icon"
    //    };
    //    return useIcon;
    //}

    //private string GetIconUrl(string iconUrl)
    //{
    //    if (iconUrl.Contains("http"))
    //        return iconUrl;
    //    else
    //        return $"./{Core.Helpers.ContentHelper.ContentPath}/css/images/{iconUrl}.png";
    //}
}
