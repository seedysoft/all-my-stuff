using Microsoft.JSInterop;

namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent : IAsyncDisposable
{
    public sealed class MapClickEventArgs : EventArgs
    {
        [J("latLng")] public required MapModels.Basic.LatLng LatLng { get; set; }
    }

    [JSInvokable] public async Task OnMapClickAsync(MapClickEventArgs args) 
        => await OnMapClickAsyncEventCallback.InvokeAsync(args);
}
