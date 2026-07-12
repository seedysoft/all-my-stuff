using Microsoft.JSInterop;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent : IAsyncDisposable
{
    #region IAsyncDisposable Impl
    public async ValueTask DisposeAsync()
    {
        await DeleteMapAsync();
        ObjRef?.Dispose();
        await MapModule.DisposeAsync();
        GC.SuppressFinalize(this);
    }
    #endregion

    public IJSObjectReference MapModule { get => field!; private set; }

    public async Task AddMarkerAsync(
        MapModels.LatLng latLng,
        MapModels.MarkerOptions? options = default,
        string? popupContent = default) => await MapModule.InvokeVoidAsync("addMarker", MapId, latLng, options, popupContent);

    public async Task DeleteMapAsync() => await MapModule.InvokeVoidAsync("destroyMap", MapId);

    public async Task RemoveAllMarkersAsync() => await MapModule.InvokeVoidAsync("removeAllMarkers", MapId);

    // Unused. Uncomment if ncccessary
    //public async Task SetViewAsync(LatLng latLng, int zoom) => await MapModule.InvokeVoidAsync("setView", MapId, latLng, zoom);
}
