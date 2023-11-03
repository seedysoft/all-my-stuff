using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Collections.Immutable;
using System.Net.Http.Json;

namespace Seedysoft.HomeCloud.Client.Pages;

public partial class RouteSearch
{
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    [Inject] private ILogger<RouteSearch> Logger { get; set; } = default!;

    private MudDataGrid<HomeCloud.Shared.ViewModels.RouteObtainedModel> RoutesMudDataGrid = default!;
    private MudForm GasStationMudForm = default!;

    private readonly HomeCloud.Shared.ViewModels.RouteQueryModel RouteQuery = new()
#if DEBUG
    {
        Origin = "Brazuelo",
        Destination = "Burgos",
    }
#endif
    ;
    private readonly HomeCloud.Shared.ViewModels.GasStationQueryModel GasStationQuery = new()
    {
        MaxDistanceInKm = 5.0M,
    }
    ;

    private IImmutableList<HomeCloud.Shared.ViewModels.IdDescRecord> PetroleumProducts = ImmutableArray<HomeCloud.Shared.ViewModels.IdDescRecord>.Empty;

    private IImmutableList<HomeCloud.Shared.ViewModels.RouteObtainedModel> RoutesObtained = ImmutableArray<HomeCloud.Shared.ViewModels.RouteObtainedModel>.Empty;

    private IImmutableList<HomeCloud.Shared.ViewModels.GasStationInfoModel> GasStations = ImmutableArray<HomeCloud.Shared.ViewModels.GasStationInfoModel>.Empty;

    private bool IsLoadingRoutes;
    private bool IsLoadingGasStations;

    public IEnumerable<HomeCloud.Shared.ViewModels.IdDescRecord> PetroleumProductsSelected { get; set; } = new HashSet<HomeCloud.Shared.ViewModels.IdDescRecord>();
    private HomeCloud.Shared.ViewModels.IdDescRecord? PetroleumProductsSelectedValue { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        string FromUri = $"{NavManager.BaseUri}{HomeCloud.Shared.ControllerUris.PetroleumProductsControllerUri}/PetroleumProductsForFilter";
        PetroleumProducts = await Http.GetFromJsonAsync<IImmutableList<HomeCloud.Shared.ViewModels.IdDescRecord>>(FromUri) ?? ImmutableArray<HomeCloud.Shared.ViewModels.IdDescRecord>.Empty;
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

    private async Task OnValidSubmitRoutesAsync(EditContext context) => await FindRoutesAsync();

    private async Task FindRoutesAsync()
    {
        IsLoadingRoutes = true;

        try
        {
            GasStations = ImmutableArray<HomeCloud.Shared.ViewModels.GasStationInfoModel>.Empty;

            string FromUri = $"{NavManager.BaseUri}{HomeCloud.Shared.ControllerUris.RoutesControllerUri}/ObtainRoutes";

            using (HttpResponseMessage Response = await Http.PostAsJsonAsync(FromUri, RouteQuery))
                RoutesObtained = (IImmutableList<HomeCloud.Shared.ViewModels.RouteObtainedModel>)(await Response.Content.ReadFromJsonAsync<IImmutableList<HomeCloud.Shared.ViewModels.RouteObtainedModel>>() ?? ImmutableArray<HomeCloud.Shared.ViewModels.RouteObtainedModel>.Empty);

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

    private string RoutesMudDataGridRowClassFunc(HomeCloud.Shared.ViewModels.RouteObtainedModel element, int rowNumber) =>
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
            string FromUri = $"{NavManager.BaseUri}{HomeCloud.Shared.ControllerUris.RoutesControllerUri}/ObtainGasStations";
            HomeCloud.Shared.ViewModels.GasStationQueryModel StationsFilter = new()
            {
                MaxDistanceInKm = GasStationQuery.MaxDistanceInKm,
                PetroleumProductsSelectedIds = PetroleumProductsSelected.Select(x => x.Id),
                Steps = RoutesMudDataGrid.SelectedItem.Steps,
            };
            using (HttpResponseMessage Response = await Http.PostAsJsonAsync(FromUri, StationsFilter))
                GasStations = await Response.Content.ReadFromJsonAsync<IImmutableList<HomeCloud.Shared.ViewModels.GasStationInfoModel>>() ?? ImmutableArray<HomeCloud.Shared.ViewModels.GasStationInfoModel>.Empty;

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
