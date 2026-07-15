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
        MapModels.Marker? marker = default,
        MapModels.Icon? icon = default,
        string? popupContent = default) => await MapModule.InvokeVoidAsync("addMarker", marker, icon, popupContent);

    public async Task DeleteMapAsync() => await MapModule.InvokeVoidAsync("destroyMap");

    public async Task RemoveAllMarkersAsync() => await MapModule.InvokeVoidAsync("removeAllMarkers");

    // Unused. Uncomment if ncccessary
    //public async Task SetViewAsync(LatLng latLng, int zoom) => await MapModule.InvokeVoidAsync("setView", latLng, zoom);
}
