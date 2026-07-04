using Microsoft.JSInterop;

namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent : IAsyncDisposable
{
    private DotNetObjectReference<MapComponent> ObjRef = default!;

    private readonly string MapId = $"map-{Guid.NewGuid()}";

    private bool IsMapReady = false;

    private async Task CreateMap(Travel.Models.Location point, byte zoomLevel = 19)
    {
        try
        {
            await LeafletService.InvokeVoidAsync("createMap", MapId, point, zoomLevel);

            if (OnMapCreatedAsync.HasDelegate)
                await OnMapCreatedAsync.InvokeAsync(this);

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

    private async Task EnableSpinnerAsync() =>
        await LeafletService.InvokeVoidAsync("enableSpinner", MapId);
    private async Task DisableSpinnerAsync() =>
        await LeafletService.InvokeVoidAsync("disableSpinner", MapId);

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
