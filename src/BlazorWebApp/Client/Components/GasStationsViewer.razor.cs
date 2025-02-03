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

    protected override Task OnInitializedAsync() => base.OnInitializedAsync();

    private string RowClassFuncion(Model element, int rowNumber)
        => element.Equals(DataGridRef.SelectedItem) ? "cursor-pointer mud-primary" : "cursor-pointer";

    //private async Task OnSelectedItemChangedLocal(Model element)
    //{
    //    if (element != null && OnSelectedItemChanged.HasDelegate)
    //        await OnSelectedItemChanged.InvokeAsync(element);
    //}
}
