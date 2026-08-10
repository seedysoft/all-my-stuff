using Microsoft.JSInterop;

namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent
{
    [JSInvokable]
    public async Task OnMapClickAsync(MapModels.Basic.LatLng args)
        => await OnMapClickAsyncEventCallback.InvokeAsync(args);

    [JSInvokable]
    public async Task OnMapMoveEndAsync(MapModels.Basic.LatLngBounds args)
    {
        await OnMapMoveEndEventCallback.InvokeAsync(args);

        if (!CurrentLatLngBounds.IsEquals(args: args, maxDegreesMargin: 0.001))
        {
            CurrentLatLngBounds = args;

            await LoadDataAsync();
        }
    }

    [JSInvokable]
    public async Task OnMarkerClickAsync(MapModels.Basic.LatLng args)
    {
        await OnCircleMarkerClicEventCallback.InvokeAsync(args);

        await ShowGasStationPopupAsync(args);
    }
}
