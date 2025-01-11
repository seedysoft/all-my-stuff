using Microsoft.AspNetCore.Components;

namespace Seedysoft.BlazorWebApp.Client.Components;

public partial class GoogleRoutesViewer
{
    [Parameter]
    public List<Libs.GoogleApis.Models.DirectionsServiceRoutes> RoutesMudTableItems { get; set; } = [];

    private MudBlazor.MudTable<Libs.GoogleApis.Models.DirectionsServiceRoutes> RoutesMudTable { get; set; } = default!;
    private int selectedRowNumber = -1;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            var i = 0;
        }
    }

    private Task RoutesMudTableOnRowClick(MudBlazor.TableRowClickEventArgs<Libs.GoogleApis.Models.DirectionsServiceRoutes> tableRowClickEventArgs)
    {
        return Task.CompletedTask;
    }

    private string SelectedRowClassFunc(Libs.GoogleApis.Models.DirectionsServiceRoutes element, int rowNumber)
    {
        // Not allow to "unselect"
        /*if (selectedRowNumber == rowNumber)
        {
            selectedRowNumber = -1;
            return string.Empty;
        }
        else*/
        if (RoutesMudTable.SelectedItem != null && RoutesMudTable.SelectedItem.Equals(element))
        {
            selectedRowNumber = rowNumber;
            return "selected";
        }
        else
        {
            return string.Empty;
        }
    }
}
