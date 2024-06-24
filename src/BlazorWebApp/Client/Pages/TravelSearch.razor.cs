using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Immutable;
using System.Net.Http.Json;

namespace Seedysoft.BlazorWebApp.Client.Pages;

public partial class TravelSearch
{
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    [Inject] private ILogger<TravelSearch> Logger { get; set; } = default!;

    private MudDataGrid<ViewModels.TravelObtainedModel> DirectionsMudDataGrid = default!;
    private MudForm GasStationMudForm = default!;

    private readonly ViewModels.TravelQueryModel TravelQuery = new()
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

    private IImmutableList<ViewModels.TravelObtainedModel> DirectionsObtained = ImmutableArray<ViewModels.TravelObtainedModel>.Empty;

    private IImmutableList<ViewModels.GasStationInfoModel> GasStations = ImmutableArray<ViewModels.GasStationInfoModel>.Empty;

    private bool IsLoadingDirections;
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

    private async Task OnValidSubmitAsync(Microsoft.AspNetCore.Components.Forms.EditContext context) => await FindDirectionsAsync();

    private async Task FindDirectionsAsync()
    {
        IsLoadingDirections = true;

        try
        {
            GasStations = ImmutableArray<ViewModels.GasStationInfoModel>.Empty;

            string FromUri = $"{NavManager.BaseUri}{ControllerUris.TravelControllerUri}/ObtainDirections";

            using (HttpResponseMessage Response = await Http.PostAsJsonAsync(FromUri, TravelQuery))
                DirectionsObtained = await Response.Content.ReadFromJsonAsync<IImmutableList<ViewModels.TravelObtainedModel>>() ?? ImmutableArray<ViewModels.TravelObtainedModel>.Empty;

            switch (DirectionsObtained.Count)
            {
                case 0:
                    _ = Snackbar.Add("No directions obtained", Severity.Warning);
                    break;
                case 1:
                    StateHasChanged();
                    await FindGasStationsAsync();
                    break;
            }
        }
        catch (Exception e) { Logger.LogError(e, "Unexpected exception"); }
        finally { IsLoadingDirections = false; }
    }

    private string DirectionsMudDataGridRowClassFunc(ViewModels.TravelObtainedModel element, int rowNumber) =>
        DirectionsMudDataGrid.SelectedItem != null && DirectionsMudDataGrid.SelectedItem.Equals(element) ? "selected" : string.Empty;

    private async void OnSubmitGasStationsAsync()
    {
        if (DirectionsMudDataGrid.SelectedItem == null)
        {
            _ = Snackbar.Add("You should select at least one direction", Severity.Error);
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
            string FromUri = $"{NavManager.BaseUri}{ControllerUris.TravelControllerUri}/ObtainGasStations";
            ViewModels.GasStationQueryModel StationsFilter = new()
            {
                MaxDistanceInKm = GasStationQuery.MaxDistanceInKm,
                PetroleumProductsSelectedIds = PetroleumProductsSelected.Select(x => x.Id),
                Steps = DirectionsMudDataGrid.SelectedItem.Steps,
            };
            using (HttpResponseMessage Response = await Http.PostAsJsonAsync(FromUri, StationsFilter))
                GasStations = await Response.Content.ReadFromJsonAsync<IImmutableList<ViewModels.GasStationInfoModel>>() ?? ImmutableArray<ViewModels.GasStationInfoModel>.Empty;

            if (!GasStations.Any())
                _ = Snackbar.Add("No gas stations obtained", Severity.Warning);
        }
        catch (Exception e) { Logger.LogError(e, "Unexpected exception"); }
        finally
        {
            IsLoadingGasStations = false;
            StateHasChanged();
        }
    }
}
