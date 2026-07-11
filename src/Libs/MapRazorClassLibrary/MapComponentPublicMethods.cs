using Microsoft.JSInterop;

namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent : IAsyncDisposable
{
    #region IAsyncDisposable Impl
    public async ValueTask DisposeAsync()
    {
        await DeleteMap();
        ObjRef?.Dispose();
        await MapModule.DisposeAsync();
        GC.SuppressFinalize(this);
    }
    #endregion

    public IJSObjectReference MapModule { get => field!; private set; }

    public async Task AddMarker(MapModels.LatLng latLng, MapModels.MarkerOptions? options = default) => await MapModule.InvokeVoidAsync("addMarker", MapId, latLng, options);

    public async Task DeleteMap() => await MapModule.InvokeVoidAsync("destroyMap", MapId);

    public async Task RemoveAllMarkers() => await MapModule.InvokeVoidAsync("removeAllMarkers", MapId);

    public async Task SetView(MapModels.LatLng latLng, int zoom) => await MapModule.InvokeVoidAsync("setView", MapId, latLng, zoom);
}
