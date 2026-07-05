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

    //public ValueTask SetViewAsync(LatLong point, byte zoomLevel = 19) =>
    //    LeafletService.InvokeVoidAsync("setView", MapId, point, zoomLevel);

    //public ValueTask<int> AddMarkerAsync(LatLong point, string title, string description, string iconUrl) =>
    //    LeafletService.InvokeAsyc<int>("addMarker", MapId, point, title, description, GetIconUrl(iconUrl));

    //public ValueTask<int> AddMarkerAsync(LatLong point, string title, string description, Icon icon = Icon.PIN) =>
    //    AddMarkerAsync(point, title, description, GetIcon(icon));

    //public ValueTask<int> AddMarkerAsync(LatLong point, string title, string description) =>
    //    AddMarkerAsync(point, title, description, "marker-icon");

    //public async ValueTask<int> AddDraggableMarkerAsync(LatLong point, string title, string description, string iconUrl)
    //{
    //    await LeafletService.InvokeVoidAsync("setMarkerHelper", MapId, ObjRef, nameof(OnDragend));
    //    return await LeafletService.InvokeAsyc<int>("addDraggableMarker", MapId, point, title, description, GetIconUrl(iconUrl));
    //}

    //public ValueTask<int> AddDraggableMarkerAsync(LatLong point, string title, string description, Icon icon = Icon.PIN) =>
    //    AddDraggableMarkerAsync(point, title, description, GetIcon(icon));

    public ValueTask RemoveMarkersAsync() =>
        MapModule.InvokeVoidAsync("removeMarkers", MapId);

    //public Task DrawCircleAsync(LatLong point, string color, string fillColor, double fillOpacity, double radius) =>
    //    LeafletService.InvokeVoidAsync("drawCircle", MapId, point, color, fillColor, fillOpacity, radius);

    public ValueTask DeleteMap() =>
        MapModule.InvokeVoidAsync("deleteMap", MapId);

    //public Task MoveMarketAsync(int markerId, LatLong newPosition) =>
    //    LeafletService.InvokeVoidAsync("moveMarker", MapId, markerId, newPosition);

    public ValueTask SetPopupMarkerContent(int markerId, string content) =>
        MapModule.InvokeVoidAsync("setPopupMarkerContent", MapId, markerId, content);

    //public static double GetDistanceInMettersBetween(LatLong origin, LatLong destination) =>
    //    Helpers.CoordinatesCalculatesHelper.CalculateDistanceInMetters(origin, destination);
}
