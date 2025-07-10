using Microsoft.AspNetCore.Components;
using RestSharp;
using Seedysoft.BlazorWebApp.Client.Extensions;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.BlazorWebApp.Client.Pages;

// TODO                 Add button to switch between Origin and Destination
// TODO                 Add button for obtain current location

public partial class TravelSearch
{
    [Inject] private ILogger<TravelSearch> Logger { get; set; } = default!;
    //[Inject] private MudBlazor.IDialogService DialogService { get; set; } = default!;
    [Inject] private MudBlazor.ISnackbar Snackbar { get; set; } = default!;
    [Inject] private IConfiguration Configuration { get; set; } = default!;
    [Inject] private Libs.GasStationPrices.Services.GasStationPricesService GasStationPricesService { get; set; } = default!;
    [Inject] private Libs.GoogleApis.Services.PlacesService PlacesService { get; set; } = default!;

    private Libs.GoogleApis.Settings.GoogleApisSettings googleApisSettings = default!;

    private Libs.GoogleMapsRazorClassLib.GoogleMap.Map TravelGoogleMap { get; set; } = default!;

    private readonly Libs.GasStationPrices.ViewModels.TravelQueryModel travelQueryModel = new()
    {
#if DEBUG
        Origin = "Calle Juan Ramon Jimenez, 8, Burgos, Spain",
        Destination = "Manciles, Spain", /*"Calle la Iglesia, 11, Brazuelo, Leon, Spain",*/
#else
        Origin = string.Empty,
        Destination = string.Empty,
#endif
        MaxDistanceInKm = 10,
        PetroleumProductsSelectedIds = Libs.GasStationPrices.Models.Minetur.ProductoPetrolifero.Gasoline.Select(static x => x.IdProducto).ToHashSet(),
    };
    private readonly Libs.GasStationPrices.ViewModels.TravelQueryModelFluentValidator travelQueryModelFluentValidator = new();

    private bool GasStationsViewerIsLoading;
    private readonly List<Libs.GasStationPrices.ViewModels.GasStationModel> GasStationItems = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        Logger.LogInformation($"Called {nameof(OnInitializedAsync)}");

        googleApisSettings = Configuration
            .GetSection(nameof(Libs.GoogleApis.Settings.GoogleApisSettings))
            .Get<Libs.GoogleApis.Settings.GoogleApisSettings>()!;
    }

    private async Task OnGasStationSelectedItemChanged(Libs.GasStationPrices.ViewModels.GasStationModel gasStationModel)
    {
        if (gasStationModel == null)
            await TravelGoogleMap.ResetViewportAsync();
        else
            await TravelGoogleMap.ClickOnMarkerAsync(gasStationModel.ToMarker());
    }

    private async Task OnClickGmapRouteAsync(string encodedPolyline)
    {
        GasStationsViewerIsLoading = true;
        GasStationItems.Clear();
        StateHasChanged();

        await foreach (Libs.GasStationPrices.ViewModels.GasStationModel gasStationModel in
            GasStationPricesService.GetNearGasStationsAsync(encodedPolyline, travelQueryModel.MaxDistanceInKm, CancellationToken.None))
        {
            GasStationItems.Add(gasStationModel);
        }

        GasStationsViewerIsLoading = false;
        StateHasChanged();
    }

    // TODO                                     Show Gas Station data
    //private void OnClickGmapMarker(Libs.GoogleMapsRazorClassLib.GoogleMap.Marker marker)
    //    => _ = Snackbar.Add($"Clicked in {marker.Content}. DateTime: {DateTime.Now}", MudBlazor.Severity.Success);
    // OnClickGmapMarkerEventCallback="@OnClickGmapMarker"

    private async Task<IEnumerable<string>> FindPlacesAsync(string textToFind, CancellationToken cancellationToken)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(textToFind))
                return await PlacesService.FindPlacesAsync(textToFind, cancellationToken) ?? [];
        }
        catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected error")) { }

        return [];
    }

    private async Task ClearDataAsync()
    {
        await TravelGoogleMap.RemoveAllMarkersAsync();
        GasStationItems.Clear();
    }

    private async Task ValidateSearch()
    {
        FluentValidation.Results.ValidationResult validationResult = await travelQueryModelFluentValidator.ValidateAsync(travelQueryModel);
        if (validationResult.IsValid)
        {
            await LoadGoogleRoutesAsync();
        }
        else
        {
            IEnumerable<string> errors = validationResult.Errors.Select(static x => $"<li>{x.ErrorMessage}</li>");
            _ = Snackbar.Add(new MarkupString($"<ul>{string.Join("", errors)}</ul>"), MudBlazor.Severity.Error);
        }
    }

    private async Task LoadGoogleRoutesAsync()
    {
        await ClearDataAsync();

        await TravelGoogleMap.SearchRoutesAsync(travelQueryModel.Origin, travelQueryModel.Destination);
    }
}
