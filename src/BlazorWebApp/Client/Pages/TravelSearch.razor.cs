using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RestSharp;
using Seedysoft.BlazorWebApp.Client.Extensions;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.BlazorWebApp.Client.Pages;

// TODO                 Add button to switch between Origin and Destination

public partial class TravelSearch
{
    [Inject] private ILogger<TravelSearch> Logger { get; set; } = default!;
    [Inject] private MudBlazor.IDialogService DialogService { get; set; } = default!;
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
        Destination = "Calle la Iglesia, 11, Brazuelo, Leon, Spain",
#else
        Origin = string.Empty,
        Destination = string.Empty,
#endif
        MaxDistanceInKm = 10,
        PetroleumProductsSelectedIds = Libs.GasStationPrices.Models.Minetur.ProductoPetrolifero.Gasoline.Select(x => x.IdProducto).ToHashSet(),
    };
    private readonly Libs.GasStationPrices.ViewModels.TravelQueryModelFluentValidator travelQueryModelFluentValidator = new();

    private readonly List<Libs.GasStationPrices.ViewModels.GasStationModel> GasStationItems = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        Logger.LogInformation($"Called {nameof(OnInitializedAsync)}");

        googleApisSettings = Configuration
            .GetSection(nameof(Libs.GoogleApis.Settings.GoogleApisSettings))
            .Get<Libs.GoogleApis.Settings.GoogleApisSettings>()!;
    }

    private void OnClickGmapMarker(Libs.GoogleMapsRazorClassLib.GoogleMap.Marker marker)
        => _ = Snackbar.Add($"Clicked in {marker.Content}. DateTime: {DateTime.Now}", MudBlazor.Severity.Success);
    private async Task OnClickGmapRouteAsync(string encodedPolyline)
    {
        GasStationItems.Clear();

        StateHasChanged();

        await foreach (Libs.GasStationPrices.ViewModels.GasStationModel gasStationModel in 
            GasStationPricesService.GetNearGasStationsAsync(encodedPolyline, travelQueryModel.MaxDistanceInKm)!)
        {
            GasStationItems.Add(gasStationModel);
            StateHasChanged();
        }
    }

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

    private async Task ShowGasStationInMapAsync(Libs.GasStationPrices.ViewModels.GasStationModel gasStationModel)
        => await TravelGoogleMap.ClickOnMarkerAsync(gasStationModel.ToMarker());

    private async Task OnPreviewInteractionAsync(MudBlazor.StepperInteractionEventArgs arg)
    {
        switch (arg.Action)
        {
            case MudBlazor.StepAction.Activate: // occurrs when clicking a step header with the mouse
                await ControlStepNavigation(arg);
                break;

            case MudBlazor.StepAction.Complete: // occurrs when clicking next
                await ControlStepCompletion(arg);

                break;

            case MudBlazor.StepAction.Reset:
            case MudBlazor.StepAction.Skip:
                break;
        }

        async Task ControlStepNavigation(MudBlazor.StepperInteractionEventArgs arg)
        {
            switch (arg.StepIndex)
            {
                case 1:
                    if (!IsStep1Complete())
                    {
                        _ = await DialogService.ShowMessageBox("Error", "Finish step 1 first");
                        arg.Cancel = true;
                    }

                    break;

                case 2:
                    if (!IsStep1Complete() || !IsStep2Complete())
                    {
                        _ = await DialogService.ShowMessageBox("Error", "Finish step 1 and 2 first");
                        arg.Cancel = true;
                    }

                    break;
            }
        }

        async Task ControlStepCompletion(MudBlazor.StepperInteractionEventArgs arg)
        {
            switch (arg.StepIndex)
            {
                case 0:
                    if (!IsStep1Complete())
                    {
                        _ = await DialogService.ShowMessageBox("Error", "You must fill Origin and Destination on first step");
                        arg.Cancel = true;
                    }

                    break;

                case 1:
                    if (!IsStep2Complete())
                    {
                        _ = await DialogService.ShowMessageBox("Error", "You must select at least one Product on second step");
                        arg.Cancel = true;
                    }

                    break;
            }
        }
    }

    private bool IsStep1Complete()
        => !string.IsNullOrWhiteSpace(travelQueryModel.Origin) && !string.IsNullOrWhiteSpace(travelQueryModel.Destination);
    private bool IsStep2Complete()
        => travelQueryModel.PetroleumProductsSelectedIds.Count > 0;

    [System.Runtime.Versioning.UnsupportedOSPlatform("browser")]
    private async Task OnCompletedChangedAsync(bool isCompleted)
    {
        if (isCompleted)
            await LoadGoogleRoutesAsync();
    }

    private async Task ClearDataAsync()
    {
        await TravelGoogleMap.RemoveAllMarkersAsync();
        GasStationItems.Clear();
        //RotuloFilterSelectedItems.Clear();
    }

    private async Task LoadGoogleRoutesAsync()
    {
        FluentValidation.Results.ValidationResult validationResult = await travelQueryModelFluentValidator.ValidateAsync(travelQueryModel);
        if (!validationResult.IsValid)
        {
            IEnumerable<string> errors = validationResult.Errors.Select(static x => $"<li>{x.ErrorMessage}</li>");
            _ = Snackbar.Add(new MarkupString($"<ul>{string.Join("", errors)}</ul>"), MudBlazor.Severity.Error);
            return;
        }

        await ClearDataAsync();

        await TravelGoogleMap.SearchRoutesAsync(travelQueryModel.Origin, travelQueryModel.Destination);
    }
}
