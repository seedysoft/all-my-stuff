using Microsoft.AspNetCore.Components;

namespace Seedysoft.BlazorWebApp.Client.Components;

public partial class GasStationsViewer
{
    [Parameter] public IEnumerable<Libs.GasStationPrices.ViewModels.GasStationModel> GasStationsMudTableItems { get; set; } = [];

    [Parameter] public bool IsLoading { get; set; }

    [Parameter] public Func<Libs.GasStationPrices.ViewModels.GasStationModel, Task>? OnItemSelected { get; set; }

    [Parameter] public IReadOnlyCollection<long> PetroleumProductsSelectedIds { get; set; } = [];

    private MudBlazor.MudTable<Libs.GasStationPrices.ViewModels.GasStationModel> GasStationsMudTable { get; set; } = default!;

    private bool IsRotuloFilterOpen = false;
    private MudBlazor.FilterDefinition<Libs.GasStationPrices.ViewModels.GasStationModel> RotuloFilterDefinition = default!;
    private HashSet<string> RotuloFilterAvailableItems => [.. GasStationsMudTableItems.Select(static x => x.RotuloTrimed).Distinct().Order()];
    private HashSet<string> RotuloFilterSelectedItems = [];
    private bool IsRotuloFilterAllSelected => RotuloFilterAvailableItems.Count == RotuloFilterSelectedItems.Count;
    public string RotuloFilterIcon => IsRotuloFilterAllSelected switch
    {
        true => MudBlazor.Icons.Material.Outlined.FilterAlt,
        false => MudBlazor.Icons.Material.Filled.FilterAlt,
    };

    protected override Task OnInitializedAsync()
    {
        RotuloFilterSelectedItems = RotuloFilterAvailableItems;
        RotuloFilterDefinition = new MudBlazor.FilterDefinition<Libs.GasStationPrices.ViewModels.GasStationModel>()
        {
            FilterFunction = x => RotuloFilterSelectedItems.Contains(x.RotuloTrimed)
        };

        return base.OnInitializedAsync();
    }

    private void RotuloFilterSelectAll(bool value)
    {
        if (value)
            RotuloFilterSelectedItems = [.. RotuloFilterAvailableItems];
        else
            RotuloFilterSelectedItems.Clear();
    }
    private void RotuloFilterSelectedChanged(bool value, string item) =>
        _ = value ? RotuloFilterSelectedItems.Add(item) : RotuloFilterSelectedItems.Remove(item);
    private async Task RotuloFilterClearAsync(
        MudBlazor.FilterContext<Libs.GasStationPrices.ViewModels.GasStationModel> gasStationModel)
    {
        RotuloFilterSelectedItems = [.. RotuloFilterAvailableItems];
        await gasStationModel.Actions.ClearFilterAsync(RotuloFilterDefinition);
        IsRotuloFilterOpen = false;
    }
    private async Task RotuloFilterApplyAsync(MudBlazor.FilterContext<Libs.GasStationPrices.ViewModels.GasStationModel> gasStationModel)
    {
        await gasStationModel.Actions.ApplyFilterAsync(RotuloFilterDefinition);
        IsRotuloFilterOpen = false;
    }

    private async Task OnRowClickGasStationsMudTable(
        MudBlazor.TableRowClickEventArgs<Libs.GasStationPrices.ViewModels.GasStationModel> tableRowClickEventArgs)
    {
        if (OnItemSelected != null && tableRowClickEventArgs.Item != null)
            await OnItemSelected.Invoke(tableRowClickEventArgs.Item);
    }

    private string SelectedRowClassFunc(
        Libs.GasStationPrices.ViewModels.GasStationModel element,
        int rowNumber)
    {
        return (GasStationsMudTable.SelectedItem != null && GasStationsMudTable.SelectedItem.Equals(element))
            ? "selected"
            : string.Empty;
    }

    private void GasStationsMudTablePageChanged(int i) => GasStationsMudTable.NavigateTo(i - 1);

    private async Task ClearDataAsync() => RotuloFilterSelectedItems.Clear();
}
