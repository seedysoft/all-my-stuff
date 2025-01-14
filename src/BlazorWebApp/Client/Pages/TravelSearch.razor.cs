using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RestSharp;
using Seedysoft.BlazorWebApp.Client.Extensions;
using Seedysoft.Libs.Core.Extensions;
using Seedysoft.Libs.GasStationPrices.Extensions;
using System.Collections.Frozen;
using System.Net.Http.Json;

namespace Seedysoft.BlazorWebApp.Client.Pages;

// TODO                 Add button to switch between Origin and Destination

public partial class TravelSearch
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private ILogger<TravelSearch> Logger { get; set; } = default!;
    [Inject] private NavigationManager NavManager { get; set; } = default!;
    [Inject] private MudBlazor.IDialogService DialogService { get; set; } = default!;
    [Inject] private MudBlazor.ISnackbar Snackbar { get; set; } = default!;
    [Inject] private IConfiguration Configuration { get; set; } = default!;
    [Inject] private Libs.GasStationPrices.Services.GasStationPricesService GasStationPricesService { get; set; } = default!;

    private Libs.GoogleApis.Settings.GoogleApisSettings googleApisSettings = default!;

    private Libs.GoogleMapsRazorClassLib.GoogleMap.Map TravelGoogleMap { get; set; } = default!;

    private Libs.GoogleMapsRazorClassLib.DirectionsService directionsService = default!;

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

    private Components.GoogleRoutesViewer GoogleRoutesViewer { get; set; } = default!;

    private MudBlazor.MudDataGrid<Libs.GasStationPrices.ViewModels.GasStationModel> GasStationsDataGrid { get; set; } = default!;
    private readonly List<Libs.GasStationPrices.ViewModels.GasStationModel> GasStationsDataGridItems = [];

    private bool IsRotuloFilterOpen = false;
    private MudBlazor.FilterDefinition<Libs.GasStationPrices.ViewModels.GasStationModel> RotuloFilterDefinition = default!;
    private HashSet<string> RotuloFilterAvailableItems => [.. GasStationsDataGridItems.Select(static x => x.RotuloTrimed).Distinct().Order()];
    private HashSet<string> RotuloFilterSelectedItems = [];
    private bool IsRotuloFilterAllSelected => RotuloFilterAvailableItems.Count == RotuloFilterSelectedItems.Count;
    public string RotuloFilterIcon => IsRotuloFilterAllSelected switch
    {
        true => MudBlazor.Icons.Material.Outlined.FilterAlt,
        false => MudBlazor.Icons.Material.Filled.FilterAlt,
    };

    private MudBlazor.MudDataGrid<Libs.GasStationPrices.ViewModels.GasStationModel> RoutesDataGrid { get; set; } = default!;
    private MudBlazor.MudDataGrid<Libs.GasStationPrices.ViewModels.GasStationModel> GasStationsDataGrid { get; set; } = default!;
    
    private bool IsRotuloFilterOpen = false;
    private MudBlazor.FilterDefinition<Libs.GasStationPrices.ViewModels.GasStationModel> RotuloFilterDefinition = default!;
    private readonly List<Libs.GasStationPrices.ViewModels.GasStationModel> GasStationsDataGridItems = [];
    private HashSet<string> RotuloFilterAvailableItems => [.. GasStationsDataGridItems.Select(static x => x.RotuloTrimed).Distinct().Order()];
    private HashSet<string> RotuloFilterSelectedItems = [];
    private bool IsRotuloFilterAllSelected => RotuloFilterAvailableItems.Count == RotuloFilterSelectedItems.Count;
    private string RotuloFilterIcon => IsRotuloFilterAllSelected switch
    {
        true => MudBlazor.Icons.Material.Outlined.FilterAlt,
        false => MudBlazor.Icons.Material.Filled.FilterAlt,
    };

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        Logger.LogInformation($"Called {nameof(OnInitializedAsync)}");

        googleApisSettings = Configuration
            .GetSection(nameof(Libs.GoogleApis.Settings.GoogleApisSettings))
            .Get<Libs.GoogleApis.Settings.GoogleApisSettings>()!;

        // Take only from Libs.GasStationPrices.Core.Json.Minetur.ProductoPetrolifero, as properties are fixed
        //string FromUri = $"{NavManager.BaseUri}{Constants.PetroleumProductsUris.Controller}/{Constants.PetroleumProductsUris.Actions.ForFilter}";
        //PetroleumProducts = await Http.GetFromJsonAsync<IEnumerable<Libs.GasStationPrices.Core.Json.Minetur.ProductoPetrolifero>>(FromUri) ?? [];

        //PetroleumProducts = File.ReadAllText("C:\\Users\\alfon\\Downloads\\RouteResponse.json").FromJson<IEnumerable<Libs.GasStationPrices.Core.Json.Minetur.ProductoPetrolifero>>();

        //FromUri = $"{NavManager.BaseUri}{Constants.TravelUris.Controller}/{Constants.TravelUris.Actions.GetMapId}";
        //string MapId = await Http.GetStringAsync(FromUri);

        RotuloFilterSelectedItems = RotuloFilterAvailableItems;
        RotuloFilterDefinition = new MudBlazor.FilterDefinition<Libs.GasStationPrices.ViewModels.GasStationModel>()
        {
            FilterFunction = x => RotuloFilterSelectedItems.Contains(x.RotuloTrimed)
        };
    }

    private void OnClickGoogleMapMarker(
        Libs.GoogleMapsRazorClassLib.GoogleMap.Marker marker)
        => _ = Snackbar.Add($"Clicked into {marker.Content}. DateTime: {DateTime.Now}", MudBlazor.Severity.Success);

    private async Task<IEnumerable<string>> FindPlacesAsync(
        string textToFind,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(textToFind))
                return [];

            string FromUri = $"{NavManager.BaseUri}{Constants.TravelUris.Controller}/{Constants.TravelUris.Actions.FindPlaces}?textToFind={textToFind}";

            return await Http.GetFromJsonAsync<IEnumerable<string>>(FromUri, cancellationToken) ?? [];
        }
        catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected error")) { }

        return [];
    }

    private void RotuloFilterSelectAll(
        bool value)
    {
        if (value)
            RotuloFilterSelectedItems = [.. RotuloFilterAvailableItems];
        else
            RotuloFilterSelectedItems.Clear();
    }
    private void RotuloFilterSelectedChanged(
        bool value, string item) =>
        _ = value ? RotuloFilterSelectedItems.Add(item) : RotuloFilterSelectedItems.Remove(item);
    private async Task RotuloFilterClearAsync(
        MudBlazor.FilterContext<Libs.GasStationPrices.ViewModels.GasStationModel> gasStationModel)
    {
        RotuloFilterSelectedItems = [.. RotuloFilterAvailableItems];
        await gasStationModel.Actions.ClearFilterAsync(RotuloFilterDefinition);
        IsRotuloFilterOpen = false;
    }
    private async Task RotuloFilterApplyAsync(
        MudBlazor.FilterContext<Libs.GasStationPrices.ViewModels.GasStationModel> gasStationModel)
    {
        await gasStationModel.Actions.ApplyFilterAsync(RotuloFilterDefinition);
        IsRotuloFilterOpen = false;
    }

    private async Task ShowGasStationInMapAsync(
        Libs.GasStationPrices.ViewModels.GasStationModel gasStationModel)
        => await TravelGoogleMap.ClickOnMarkerAsync(gasStationModel.ToMarker());

    private async Task OnPreviewInteractionAsync(
        MudBlazor.StepperInteractionEventArgs arg)
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
    private async Task OnCompletedChangedAsync(
        bool isCompleted)
    {
        if (!isCompleted)
            return;

        await LoadGoogleRoutesAsync();
    }

    private async Task ClearDataAsync()
    {
        await TravelGoogleMap.RemoveAllMarkersAsync();
        GoogleRoutesViewer.RoutesMudTableItems.Clear();
        GasStationsDataGridItems.Clear();
        RotuloFilterSelectedItems.Clear();
    }

    [System.Runtime.Versioning.UnsupportedOSPlatform("browser")]
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

        Libs.GoogleApis.Models.Directions.Request.Body directionsRequest = new()
        {
            Origin = travelQueryModel.Origin,
            Destination = travelQueryModel.Destination,
            TravelMode = Libs.GoogleApis.Models.Directions.Shared.TravelMode.Driving,
            //AvoidFerries = false,
            //AvoidHighways = false,
            //AvoidTolls = false,
            //DrivingOptions = new() { DepartureTime = DateTime.UtcNow, TrafficModel = Libs.GoogleMapsRazorClassLib.Directions.TrafficModel.bestguess, },
            //OptimizeWaypoints = false,
            ProvideRouteAlternatives = true,
            ////Region = "es",
            ////TransitOptions = new()
            ////{
            ////    ArrivalTime = DateTime.UtcNow,
            ////    DepartureTime = DateTime.UtcNow,
            ////    Modes = [GoogleMapsComponents.Maps.TransitMode.Bus, GoogleMapsComponents.Maps.TransitMode.Train],
            ////    RoutingPreference = GoogleMapsComponents.Maps.TransitRoutePreference.FewerTransfers,
            ////},
            ////UnitSystem = Libs.GoogleMapsRazorClassLib.Directions.UnitSystem.Metric, // InvalidValueError: in property unitSystem: metric is not an accepted value
            //Waypoints = [
            //    //new GoogleMapsComponents.Maps.DirectionsWaypoint() { Location = "Bethlehem, PA", Stopover = true }
            //],
        };
        directionsService ??= new(JSRuntime, TravelGoogleMap.Id);
        GoogleRoutesViewer.RoutesMudTableItems.AddRange(await directionsService.RouteAsync(directionsRequest));
    }

    private async Task RoutesDataGridItemSelectedAsync(
        Libs.GoogleApis.Models.DirectionsServiceRoutes directionsServiceRoutes)
    {
        IReadOnlySet<Libs.GoogleApis.Models.Shared.LatLngLiteral> Points = await TravelGoogleMap.HighlightRouteAsync(directionsServiceRoutes.Index);

        if (!Points.Any())
            throw new ApplicationException($"{nameof(Libs.GoogleApis.Services.RoutesService.GetRoutesAsync)} does not finds route points");

        GasStationsDataGridItems.AddRange(await GasStationPricesService.GetNearGasStationsAsync(Points, travelQueryModel.MaxDistanceInKm));

        RotuloFilterSelectedItems = [.. RotuloFilterAvailableItems];
    }

    private void RotuloFilterSelectAll(
        bool value)
    {
        if (value)
            RotuloFilterSelectedItems = [.. RotuloFilterAvailableItems];
        else
            RotuloFilterSelectedItems.Clear();
    }
    private void RotuloFilterSelectedChanged(
        bool value, string item) =>
        _ = value ? RotuloFilterSelectedItems.Add(item) : RotuloFilterSelectedItems.Remove(item);
    private async Task RotuloFilterClearAsync(
        MudBlazor.FilterContext<Libs.GasStationPrices.ViewModels.GasStationModel> gasStationModel)
    {
        RotuloFilterSelectedItems = [.. RotuloFilterAvailableItems];
        await gasStationModel.Actions.ClearFilterAsync(RotuloFilterDefinition);
        IsRotuloFilterOpen = false;
    }
    private async Task RotuloFilterApplyAsync(
        MudBlazor.FilterContext<Libs.GasStationPrices.ViewModels.GasStationModel> gasStationModel)
    {
        await gasStationModel.Actions.ApplyFilterAsync(RotuloFilterDefinition);
        IsRotuloFilterOpen = false;
    }

    private void ShowGasStationInMap(
        Libs.GasStationPrices.ViewModels.GasStationModel gasStationModel)
        => TravelGoogleMap.ClickOnMarker(gasStationModel.ToMarker());
}
