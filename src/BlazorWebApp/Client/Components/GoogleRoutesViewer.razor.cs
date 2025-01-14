using Microsoft.AspNetCore.Components;

namespace Seedysoft.BlazorWebApp.Client.Components;

public partial class GoogleRoutesViewer
{
    [Parameter] public List<Libs.GoogleApis.Models.DirectionsServiceRoutes> RoutesMudTableItems { get; set; } = [];

    [Parameter] public Func<Libs.GoogleApis.Models.DirectionsServiceRoutes, Task>? OnRouteSelected { get; set; }

    private MudBlazor.MudTable<Libs.GoogleApis.Models.DirectionsServiceRoutes> RoutesMudTable { get; set; } = default!;

    private async Task RoutesMudTableOnRowClick(
        MudBlazor.TableRowClickEventArgs<Libs.GoogleApis.Models.DirectionsServiceRoutes> tableRowClickEventArgs)
    {
        if (OnRouteSelected != null && tableRowClickEventArgs.Item != null)
            await OnRouteSelected.Invoke(tableRowClickEventArgs.Item);
    }

    private string SelectedRowClassFunc(
        Libs.GoogleApis.Models.DirectionsServiceRoutes element,
        int rowNumber)
    {
        return (RoutesMudTable.SelectedItem != null && RoutesMudTable.SelectedItem.Equals(element))
            ? "selected"
            : string.Empty;
    }
}
