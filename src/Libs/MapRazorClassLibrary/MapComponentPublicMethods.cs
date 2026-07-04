namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent : IAsyncDisposable
{
    //public Task SetViewAsync(LatLong point, byte zoomLevel = 19) =>
    //    LeafletService.InvokeVoidAsync("setView", MapId, point, zoomLevel);

    //public Task<int> AddMarkerAsync(LatLong point, string title, string description, string iconUrl) =>
    //    LeafletService.InvokeAsyc<int>("addMarker", MapId, point, title, description, GetIconUrl(iconUrl));

    //public Task<int> AddMarkerAsync(LatLong point, string title, string description, Icon icon = Icon.PIN) =>
    //    AddMarkerAsync(point, title, description, GetIcon(icon));

    //public Task<int> AddMarkerAsync(LatLong point, string title, string description) =>
    //    AddMarkerAsync(point, title, description, "marker-icon");

    //public async Task<int> AddDraggableMarkerAsync(LatLong point, string title, string description, string iconUrl)
    //{
    //    await LeafletService.InvokeVoidAsync("setMarkerHelper", MapId, ObjRef, nameof(OnDragend));
    //    return await LeafletService.InvokeAsyc<int>("addDraggableMarker", MapId, point, title, description, GetIconUrl(iconUrl));
    //}

    //public Task<int> AddDraggableMarkerAsync(LatLong point, string title, string description, Icon icon = Icon.PIN) =>
    //    AddDraggableMarkerAsync(point, title, description, GetIcon(icon));

    public Task RemoveMarkersAsync() =>
        LeafletService.InvokeVoidAsync("removeMarkers", MapId);

    //public Task DrawCircleAsync(LatLong point, string color, string fillColor, double fillOpacity, double radius) =>
    //    LeafletService.InvokeVoidAsync("drawCircle", MapId, point, color, fillColor, fillOpacity, radius);

    public Task DeleteMap() =>
        LeafletService.InvokeVoidAsync("deleteMap", MapId);

    //public Task MoveMarketAsync(int markerId, LatLong newPosition) =>
    //    LeafletService.InvokeVoidAsync("moveMarker", MapId, markerId, newPosition);

    public Task SetPopupMarkerContent(int markerId, string content) =>
        LeafletService.InvokeVoidAsync("setPopupMarkerContent", MapId, markerId, content);
    public async ValueTask DisposeAsync()
    {
        await LeafletService.InvokeVoidAsync("deleteMap", MapId);
        ObjRef?.Dispose();
        GC.SuppressFinalize(this);
    }

    //public static double GetDistanceInMettersBetween(LatLong origin, LatLong destination) =>
    //    Helpers.CoordinatesCalculatesHelper.CalculateDistanceInMetters(origin, destination);
}
