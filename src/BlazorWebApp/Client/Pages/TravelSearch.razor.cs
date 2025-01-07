using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Seedysoft.BlazorWebApp.Client.Extensions;
using Seedysoft.Libs.Core.Extensions;
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

    private Libs.GasStationPrices.Settings.GasStationPricesSettings gasStationPricesSettings = default!;
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

    private MudBlazor.MudDataGrid<Libs.GoogleApis.Models.DirectionsServiceRoutes> RoutesDataGrid { get; set; } = default!;
    private readonly List<Libs.GoogleApis.Models.DirectionsServiceRoutes> RoutesDataGridItems = [];

    private MudBlazor.MudDataGrid<Libs.GasStationPrices.ViewModels.GasStationModel> GasStationsDataGrid { get; set; } = default!;
    private readonly List<Libs.GasStationPrices.ViewModels.GasStationModel> GasStationsDataGridItems = [];

    private bool IsRotuloFilterOpen = false;
    private MudBlazor.FilterDefinition<Libs.GasStationPrices.ViewModels.GasStationModel> RotuloFilterDefinition = default!;
    private HashSet<string> RotuloFilterAvailableItems => [.. GasStationsDataGridItems.Select(static x => x.RotuloTrimed).Distinct().Order()];
    private HashSet<string> RotuloFilterSelectedItems = [];
    private bool IsRotuloFilterAllSelected => RotuloFilterAvailableItems.Count == RotuloFilterSelectedItems.Count;
    private string RotuloFilterIcon => IsRotuloFilterAllSelected switch
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

        gasStationPricesSettings = Configuration
            .GetSection(nameof(Libs.GasStationPrices.Settings.GasStationPricesSettings)).Get<Libs.GasStationPrices.Settings.GasStationPricesSettings>()!;
        googleApisSettings = Configuration
            .GetSection(nameof(Libs.GoogleApis.Settings.GoogleApisSettings)).Get<Libs.GoogleApis.Settings.GoogleApisSettings>()!;

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

    private void OnGoogleMapMarkerClick(
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

    private async Task ShowDirectionsServiceRouteInMapAsync(
        int directionsServiceRouteIndex)
        => await TravelGoogleMap.HighlightRouteAsync(directionsServiceRouteIndex);

    private async Task ShowGasStationInMapAsync(
        Libs.GasStationPrices.ViewModels.GasStationModel gasStationModel)
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

    private bool IsStep1Complete() => !string.IsNullOrWhiteSpace(travelQueryModel.Origin) && !string.IsNullOrWhiteSpace(travelQueryModel.Destination);
    private bool IsStep2Complete() => travelQueryModel.PetroleumProductsSelectedIds.Count > 0;

    [System.Runtime.Versioning.UnsupportedOSPlatform("browser")]
    private async Task OnCompletedChangedAsync(bool isCompleted)
    {
        if (!isCompleted)
            return;

        await LoadDataAsync();
    }

    private async Task ClearDataAsync()
    {
        await TravelGoogleMap.RemoveAllMarkersAsync();
        RoutesDataGridItems.Clear();
        GasStationsDataGridItems.Clear();
        RotuloFilterSelectedItems.Clear();
    }

    [System.Runtime.Versioning.UnsupportedOSPlatform("browser")]
    private async Task LoadDataAsync()
    {
        FluentValidation.Results.ValidationResult validationResult = await travelQueryModelFluentValidator.ValidateAsync(travelQueryModel);
        if (!validationResult.IsValid)
        {
            IEnumerable<string> errors = validationResult.Errors.Select(static x => $"<li>{x.ErrorMessage}</li>");
            _ = Snackbar.Add(new MarkupString($"<ul>{string.Join("", errors)}</ul>"), MudBlazor.Severity.Error);
            return;
        }

        await ClearDataAsync();

        //Direction Request
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

        RoutesDataGridItems.AddRange(await directionsService.RouteAsync(directionsRequest));

        //// Load GasStations
        //StringContent requestContent = new(
        //    System.Text.Json.JsonSerializer.Serialize(travelQueryModel),
        //    System.Text.Encoding.UTF8,
        //    System.Net.Mime.MediaTypeNames.Application.Json);
        //string FromUri = $"{NavManager.BaseUri}{Constants.TravelUris.Controller}/{Constants.TravelUris.Actions.GetGasStations}";
        //HttpRequestMessage requestMessage = new(HttpMethod.Post, FromUri) { Content = requestContent, };

        //HttpResponseMessage response = await Http.SendAsync(requestMessage);

        //if (!response.IsSuccessStatusCode)
        //    return;

        //await foreach (Libs.GasStationPrices.ViewModels.GasStationModel? gasStationModel in
        //    response.Content.ReadFromJsonAsAsyncEnumerable<Libs.GasStationPrices.ViewModels.GasStationModel>()!)
        //{
        //    if (gasStationModel == null)
        //        continue;

        //    GasStationsDataGridItems.Add(gasStationModel);

        //    // TODO                             Fix markers

        //    //await TravelGoogleMap.AddMarkerAsync(new Libs.GoogleMapsRazorClassLib.GoogleMap.Marker()
        //    //{
        //    //    Content = $"<div style='background-color:blue'>{gasStationModel.RotuloTrimed}</div>",
        //    //    PinElement = new()
        //    //    {
        //    //        Background = "",
        //    //        BorderColor = "",
        //    //        Glyph = null,
        //    //        GlyphColor = "azure",
        //    //        Scale = 1.0,
        //    //        UseIconFonts = true,
        //    //    },
        //    //    Position = new(gasStationModel.Lat, gasStationModel.Lng),
        //    //    Title = gasStationModel.RotuloTrimed,
        //    //});
        //}

        //RotuloFilterSelectedItems = [.. RotuloFilterAvailableItems];
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
