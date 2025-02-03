using Microsoft.AspNetCore.Components;

using Model = Seedysoft.Libs.GasStationPrices.ViewModels.GasStationModel;

namespace Seedysoft.BlazorWebApp.Client.Components;

public partial class GasStationsViewer
{
    #region Parameters

    [Parameter] public IEnumerable<Model> Elements { get; set; } = [];

    [Parameter] public bool IsLoading { get; set; }

    [Parameter] public EventCallback<Model> OnSelectedItemChanged { get; set; } = default!;

    [Parameter] public IReadOnlyCollection<long> PetroleumProductsSelectedIds { get; set; } = [];

    #endregion

    private MudBlazor.MudDataGrid<Model> DataGridRef { get; set; } = default!;

    protected override Task OnInitializedAsync()
    {
        RotuloFilterSelectedItems = RotuloFilterAvailableItems;
        RotuloFilterDefinition = new MudBlazor.FilterDefinition<Model>()
        {
            FilterFunction = x => RotuloFilterSelectedItems.Contains(x.RotuloTrimed)
        };

        return base.OnInitializedAsync();
    }

    private string RowClassFuncion(Model element, int rowNumber)
        => element.Equals(DataGridRef.SelectedItem) ? "cursor-pointer mud-primary" : "cursor-pointer";

    #region RotuloFiltering

    private bool IsRotuloFilterOpen = false;
    private MudBlazor.FilterDefinition<Model> RotuloFilterDefinition = default!;
    private SortedSet<string> RotuloFilterAvailableItems => [.. Elements.Select(static x => x.RotuloTrimed).Distinct()];
    private SortedSet<string> RotuloFilterSelectedItems = [];
    private bool IsRotuloFilterAllSelected => RotuloFilterAvailableItems.Count == RotuloFilterSelectedItems.Count;
    public string RotuloFilterIcon => IsRotuloFilterAllSelected switch
    {
        true => MudBlazor.Icons.Material.Outlined.FilterList,
        false => MudBlazor.Icons.Material.Filled.FilterList,
    };

    private void RotuloFilterSelectAll(bool value)
    {
        if (value)
            RotuloFilterSelectedItems = [.. RotuloFilterAvailableItems];
        else
            RotuloFilterSelectedItems.Clear();
    }

    private void RotuloFilterSelectedChanged(bool value, string item) =>
       _ = value ? RotuloFilterSelectedItems.Add(item) : RotuloFilterSelectedItems.Remove(item);

    private async Task RotuloFilterClearAsync(MudBlazor.FilterContext<Model> gasStationModel)
    {
        IsLoading = true;
        RotuloFilterSelectedItems = [.. RotuloFilterAvailableItems];
        await gasStationModel.Actions.ClearFilterAsync(RotuloFilterDefinition);
        IsRotuloFilterOpen = false;
        IsLoading = false;
    }

    private async Task RotuloFilterApplyAsync(MudBlazor.FilterContext<Model> gasStationModel)
    {
        IsLoading = true;
        await gasStationModel.Actions.ApplyFilterAsync(RotuloFilterDefinition);
        IsRotuloFilterOpen = false;
        IsLoading = false;
    }

    #endregion
}
