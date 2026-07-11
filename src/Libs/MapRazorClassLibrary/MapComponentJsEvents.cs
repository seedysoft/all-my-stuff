using Microsoft.JSInterop;
using Seedysoft.Libs.MapRazorClassLibrary.MapModels;

namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent : IAsyncDisposable
{
    public sealed class MapClickEventArgs : EventArgs
    {
        [J("latLng")] public required LatLng LatLng { get; set; }
    }

    [JSInvokable] public async Task OnMapClick(MapClickEventArgs args) => await OnMapClickAsync.InvokeAsync(args);
}
