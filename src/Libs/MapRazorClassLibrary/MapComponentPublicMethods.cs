namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent : IAsyncDisposable
{
    private async Task CreateMap(ILatLong point, byte zoomLevel = 19)
    {
        try
        {
            await LeafletService.InvokeVoidAsync("createMap", MapId, point, zoomLevel);
            if (OnMapCreatedAsync.HasDelegate)
                await OnMapCreatedAsync.InvokeAsync(this);
            if (OnMapClickAsync.HasDelegate)
            {
                await LeafletService.InvokeVoidAsync("setClickHandler", MapId, ObjRef, nameof(OnMapClick));
            }
        }
        catch (Exception ex)
        {
            await Console.Out.WriteAsync(ex.Message);
        }
        IsMapReady = true;
        await InvokeAsync(StateHasChanged);
    }

    public Task SetViewAsync(ILatLong point, byte zoomLevel = 19) =>
        LeafletService.InvokeVoidAsync("setView", MapId, point, zoomLevel);

    public Task<int> AddMarkerAsync(ILatLong point, string title, string description, string iconUrl)
    {
        return LeafletService.InvokeAsyc<int>("addMarker", MapId, point, title, description, GetIconUrl(iconUrl));
    }


    public Task<int> AddMarkerAsync(ILatLong point, string title, string description, Icon icon = Icon.PIN) =>
        AddMarkerAsync(point, title, description, GetIcon(icon));

    public Task<int> AddMarkerAsync(ILatLong point, string title, string description) =>
        AddMarkerAsync(point, title, description, "marker-icon");

    public async Task<int> AddDraggableMarkerAsync(ILatLong point, string title, string description, string iconUrl)
    {
        await LeafletService.InvokeVoidAsync("setMarkerHelper", MapId, ObjRef, nameof(OnDragend));
        return await LeafletService.InvokeAsyc<int>("addDraggableMarker", MapId, point, title, description, GetIconUrl(iconUrl));
    }

    public Task<int> AddDraggableMarkerAsync(ILatLong point, string title, string description, Icon icon = Icon.PIN) =>
        AddDraggableMarkerAsync(point, title, description, GetIcon(icon));

    public Task RemoveMarkersAsync() =>
        LeafletService.InvokeVoidAsync("removeMarkers", MapId);

    public Task DrawCircleAsync(ILatLong point, string color, string fillColor, double fillOpacity, double radius) =>
        LeafletService.InvokeVoidAsync("drawCircle", MapId, point, color, fillColor, fillOpacity, radius);

    public Task DeleteMap() =>
        LeafletService.InvokeVoidAsync("deleteMap", MapId);

    public Task MoveMarketAsync(int markerId, ILatLong newPosition) =>
        LeafletService.InvokeVoidAsync("moveMarker", MapId, markerId, newPosition);

    public Task SetPopupMarkerContent(int markerId, string content) =>
        LeafletService.InvokeVoidAsync("setPopupMarkerContent", MapId, markerId, content);

    public double GetDistanceInMettersBetween(ILatLong origin, ILatLong destination)
    {
        CoordinatesCalculatesHelper calculates = new CoordinatesCalculatesHelper();
        return calculates.CalculateDistanceInMetters(origin, destination);
    }
}
