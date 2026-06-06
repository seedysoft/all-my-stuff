using Microsoft.AspNetCore.Components;
using Seedysoft.Libs.Core.Extensions;
using Seedysoft.Libs.Geography.ViewModels;

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
    [Inject] private Libs.Geography.Services.PlacesService PlacesService { get; set; } = default!;

    //private Libs.GoogleApis.Settings.GoogleApisSettings googleApisSettings = default!;

    private Libs.MapRazorClassLibrary.MapComponent TravelMap { get; set; } = default!;

    private readonly TravelQueryModel travelQueryModel = new()
    {
        Origin = new Place()
        {
#if DEBUG
            Address = "Calle Juan Ramon Jimenez, 8, Burgos, Spain",
            Latitude = 42.3485f,
            Longitude = -3.6962f,
#else
            Address = "",
            Latitude = 0f,
            Longitude = 0f,
#endif
        },
        Destination = new Place()
        {
#if DEBUG
            Address = "Manciles, Spain",
            Latitude = 42.5996f,
            Longitude = -5.6684f
        },
#else
            Address = "",
            Latitude = 0f,
            Longitude = 0f,
#endif
        MaxDistanceInKm = 10,
        PetroleumProductsSelectedIds = Libs.GasStationPrices.Models.Minetur.ProductoPetrolifero.Gasoline.Select(static x => x.IdProducto).ToHashSet(),
    };
    private readonly TravelQueryModelFluentValidator travelQueryModelFluentValidator = new();

    private bool GasStationsViewerIsLoading;
    private readonly List<Libs.GasStationPrices.ViewModels.GasStationModel> GasStationItems = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (Logger.IsEnabled(LogLevel.Information))
            Logger.LogInformation($"Called {nameof(OnInitializedAsync)}");
    }

    private async Task OnGasStationSelectedItemChanged(Libs.GasStationPrices.ViewModels.GasStationModel gasStationModel)
    {
        //if (gasStationModel == null)
        //    await TravelMap.ResetViewportAsync();
        //else
        //    await TravelMap.ClickOnMarkerAsync(gasStationModel.ToMarker(travelQueryModel.PetroleumProductsSelectedIds));
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

    private async Task<IEnumerable<Place>> FindPlacesAsync(string textToFind, CancellationToken cancellationToken)
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
        TravelMap.ClearMap();

        GasStationItems.Clear();

        await Task.CompletedTask;
    }

    private async Task ValidateSearch()
    {
        FluentValidation.Results.ValidationResult validationResult = await travelQueryModelFluentValidator.ValidateAsync(travelQueryModel);
        if (validationResult.IsValid)
        {
            await LoadRoutesAsync();
        }
        else
        {
            IEnumerable<string> errors = validationResult.Errors.Select(static x => $"<li>{x.ErrorMessage}</li>");
            _ = Snackbar.Add(new MarkupString($"<ul>{string.Join("", errors)}</ul>"), MudBlazor.Severity.Error);
        }
    }

    private async Task LoadRoutesAsync()
    {
        await ClearDataAsync();

        await TravelMap.SearchRoutesAsync(travelQueryModel);
    }
}
