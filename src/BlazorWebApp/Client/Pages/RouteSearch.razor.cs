using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Immutable;
using System.Net.Http.Json;

namespace Seedysoft.BlazorWebApp.Client.Pages;

public partial class RouteSearch
{
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    [Inject] private ILogger<RouteSearch> Logger { get; set; } = default!;

    private MudDataGrid<ViewModels.RouteObtainedModel> RoutesMudDataGrid = default!;
    private MudForm GasStationMudForm = default!;

    private readonly ViewModels.RouteQueryModel RouteQuery = new()
#if DEBUG
    {
        Origin = "Brazuelo",
        Destination = "Burgos",
    }
#endif
    ;
    private readonly ViewModels.GasStationQueryModel GasStationQuery = new()
    {
        MaxDistanceInKm = 5.0M,
    }
    ;

    private IImmutableList<ViewModels.IdDescRecord> PetroleumProducts = ImmutableArray<ViewModels.IdDescRecord>.Empty;

    private IImmutableList<ViewModels.RouteObtainedModel> RoutesObtained = ImmutableArray<ViewModels.RouteObtainedModel>.Empty;

    private IImmutableList<ViewModels.GasStationInfoModel> GasStations = ImmutableArray<ViewModels.GasStationInfoModel>.Empty;

    private bool IsLoadingRoutes;
    private bool IsLoadingGasStations;

    public IEnumerable<ViewModels.IdDescRecord> PetroleumProductsSelected { get; set; } = new HashSet<ViewModels.IdDescRecord>();
    private ViewModels.IdDescRecord? PetroleumProductsSelectedValue { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        string FromUri = $"{NavManager.BaseUri}{ControllerUris.PetroleumProductsControllerUri}/PetroleumProductsForFilter";
        PetroleumProducts = await Http.GetFromJsonAsync<IImmutableList<ViewModels.IdDescRecord>>(FromUri) ?? ImmutableArray<ViewModels.IdDescRecord>.Empty;
        PetroleumProductsSelected = PetroleumProducts;
    }

    private string GetMultiSelectionText(List<string> selectedValues)
    {
        Logger.LogDebug("{SelectedValues} values on {GetMultiSelectionText}", selectedValues, nameof(GetMultiSelectionText));

        return selectedValues.Count switch
        {
            0 => "No product selected",
            1 => "One product selected",
            _ => $"{selectedValues.Count} products selected",
        };
    }

    private async Task OnValidSubmitRoutesAsync(Microsoft.AspNetCore.Components.Forms.EditContext context) => await FindRoutesAsync();

    private async Task FindRoutesAsync()
    {
        IsLoadingRoutes = true;

        try
        {
            GasStations = ImmutableArray<ViewModels.GasStationInfoModel>.Empty;

            string FromUri = $"{NavManager.BaseUri}{ControllerUris.RoutesControllerUri}/ObtainRoutes";

            using (HttpResponseMessage Response = await Http.PostAsJsonAsync(FromUri, RouteQuery))
                RoutesObtained = (IImmutableList<ViewModels.RouteObtainedModel>)(await Response.Content.ReadFromJsonAsync<IImmutableList<ViewModels.RouteObtainedModel>>() ?? ImmutableArray<ViewModels.RouteObtainedModel>.Empty);

            switch (RoutesObtained.Count)
            {
                case 0:
                    _ = Snackbar.Add("No routes obtained", Severity.Warning);
                    break;
                case 1:
                    StateHasChanged();
                    await FindGasStationsAsync();
                    break;
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Unexpected exception");
        }
        finally
        {
            IsLoadingRoutes = false;
        }
    }

    private string RoutesMudDataGridRowClassFunc(ViewModels.RouteObtainedModel element, int rowNumber) =>
        RoutesMudDataGrid.SelectedItem != null && RoutesMudDataGrid.SelectedItem.Equals(element) ? "selected" : string.Empty;

    private async void OnSubmitGasStationsAsync()
    {
        if (RoutesMudDataGrid.SelectedItem == null)
        {
            _ = Snackbar.Add("You should select at least one route", Severity.Error);
            return;
        }

        await GasStationMudForm.Validate();

        if (GasStationMudForm.IsValid)
            await FindGasStationsAsync();
    }

    private async Task FindGasStationsAsync()
    {
        IsLoadingGasStations = true;

        try
        {
            string FromUri = $"{NavManager.BaseUri}{ControllerUris.RoutesControllerUri}/ObtainGasStations";
            ViewModels.GasStationQueryModel StationsFilter = new()
            {
                MaxDistanceInKm = GasStationQuery.MaxDistanceInKm,
                PetroleumProductsSelectedIds = PetroleumProductsSelected.Select(x => x.Id),
                Steps = RoutesMudDataGrid.SelectedItem.Steps,
            };
            using (HttpResponseMessage Response = await Http.PostAsJsonAsync(FromUri, StationsFilter))
                GasStations = await Response.Content.ReadFromJsonAsync<IImmutableList<ViewModels.GasStationInfoModel>>() ?? ImmutableArray<ViewModels.GasStationInfoModel>.Empty;

            if (!GasStations.Any())
                _ = Snackbar.Add("No gas stations obtained", Severity.Warning);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Unexpected exception");
        }
        finally
        {
            IsLoadingGasStations = false;
            StateHasChanged();
        }
    }
}
