using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Seedysoft.Libs.Core.Extensions;
using System.Net.Http.Json;

namespace Seedysoft.BlazorWebApp.Client.Pages;

public partial class TravelSearch
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private ILogger<TravelSearch> Logger { get; set; } = default!;
    [Inject] private NavigationManager NavManager { get; set; } = default!;
    [Inject] private MudBlazor.ISnackbar Snackbar { get; set; } = default!;
    [Inject] private IConfiguration Configuration { get; set; } = default!;

    private Libs.GasStationPrices.Settings.GasStationPricesSettings gasStationPricesSettings = default!;
    private Libs.GoogleApis.Settings.GoogleApisSettings googleApisSettings = default!;

    private MudBlazor.MudForm TravelQueryMudForm { get; set; } = default!;
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
        MaxDistanceInKm = 5,
        PetroleumProductsSelectedIds = [],
    };
    private readonly Libs.GasStationPrices.ViewModels.TravelQueryModelFluentValidator travelQueryModelFluentValidator = new();

    private readonly IEnumerable<Libs.GasStationPrices.Models.Minetur.ProductoPetrolifero> PetroleumProducts =
         Libs.GasStationPrices.Models.Minetur.ProductoPetrolifero.All;

    protected override async Task OnInitializedAsync()
    {
        Logger.LogInformation($"Called {nameof(OnInitializedAsync)}");

        gasStationPricesSettings = Configuration
            .GetSection(nameof(Libs.GasStationPrices.Settings.GasStationPricesSettings))
            .Get<Libs.GasStationPrices.Settings.GasStationPricesSettings>()!;
        googleApisSettings = Configuration
            .GetSection(nameof(Libs.GoogleApis.Settings.GoogleApisSettings))
            .Get<Libs.GoogleApis.Settings.GoogleApisSettings>()!;

        await base.OnInitializedAsync();

        // Take only from Libs.GasStationPrices.Core.Json.Minetur.ProductoPetrolifero, as properties are fixed
        //string FromUri = $"{NavManager.BaseUri}{Constants.PetroleumProductsUris.Controller}/{Constants.PetroleumProductsUris.Actions.ForFilter}";
        //PetroleumProducts = await Http.GetFromJsonAsync<IEnumerable<Libs.GasStationPrices.Core.Json.Minetur.ProductoPetrolifero>>(FromUri) ?? [];

        //PetroleumProducts = File.ReadAllText("C:\\Users\\alfon\\Downloads\\RouteResponse.json").FromJson<IEnumerable<Libs.GasStationPrices.Core.Json.Minetur.ProductoPetrolifero>>();

        travelQueryModel.PetroleumProductsSelectedIds = PetroleumProducts
            .Where(static x => x.Abreviatura.StartsWith("G9"))
            .Select(static x => x.IdProducto)
            .ToArray()
            .AsReadOnly();

        //FromUri = $"{NavManager.BaseUri}{Constants.TravelUris.Controller}/{Constants.TravelUris.Actions.GetMapId}";
        //string MapId = await Http.GetStringAsync(FromUri);
    }

    //protected override Task OnAfterRenderAsync(bool firstRender)
    //{
    //    Logger.LogInformation("Called {method}. Is First Render: {firstRender}", nameof(OnAfterRenderAsync), firstRender);

    //    return base.OnAfterRenderAsync(firstRender);
    //}

    //protected override bool ShouldRender()
    //{
    //    bool shouldRender = base.ShouldRender();

    //    Logger.LogInformation("Called {method}: {shouldRender}", nameof(ShouldRender), shouldRender);

    //    if (shouldRender)
    //    {

    //    }

    //    return shouldRender;
    //}

    private void OnGoogleMapMarkerClick(Libs.GoogleMapsRazorClassLib.GoogleMap.Marker marker)
        => _ = Snackbar.Add($"Clicked into {marker.Content}. DateTime: {DateTime.Now}", MudBlazor.Severity.Success);

    private async Task<IEnumerable<string>> FindPlacesAsync(
        string textToFind,
        CancellationToken cancellationToken)
    {
        try
        {
            // TODO             CACHE?
            if (string.IsNullOrWhiteSpace(textToFind))
                return [];

            string FromUri = $"{NavManager.BaseUri}{Constants.TravelUris.Controller}/{Constants.TravelUris.Actions.FindPlaces}?textToFind={textToFind}";

            return await Http.GetFromJsonAsync<IEnumerable<string>>(FromUri, cancellationToken) ?? [];
        }
        catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected error")) { }

        return [];
    }

    private void SelectAllChips()
        => travelQueryModel.PetroleumProductsSelectedIds = PetroleumProducts.Select(static x => x.IdProducto).ToArray().AsReadOnly();
    private void UnSelectAllChips()
        => travelQueryModel.PetroleumProductsSelectedIds = [];

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

        await TravelGoogleMap.RemoveAllMarkersAsync();

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
        //travelQueryModel.Bounds = await directionsService.RouteAsync(directionsRequest);
        await directionsService.RouteAsync(directionsRequest);

        // Load GasStations
        StringContent requestContent = new(
            System.Text.Json.JsonSerializer.Serialize(travelQueryModel),
            System.Text.Encoding.UTF8,
            System.Net.Mime.MediaTypeNames.Application.Json);
        string FromUri = $"{NavManager.BaseUri}{Constants.TravelUris.Controller}/{Constants.TravelUris.Actions.GetGasStations}";
        HttpRequestMessage requestMessage = new(HttpMethod.Post, FromUri) { Content = requestContent, };

        HttpResponseMessage response = await Http.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
            return;

        await foreach (Libs.GasStationPrices.ViewModels.GasStationModel? gasStationModel in
            response.Content.ReadFromJsonAsAsyncEnumerable<Libs.GasStationPrices.ViewModels.GasStationModel>()!)
        {
            if (gasStationModel == null)
                continue;

            // TODO                             Fix markers

            await TravelGoogleMap.AddMarkerAsync(new Libs.GoogleMapsRazorClassLib.GoogleMap.Marker()
            {
                Content = $"<div style='background-color:blue'>{gasStationModel.Rotulo}</div>",
                PinElement = new()
                {
                    Background = "",
                    BorderColor = "",
                    Glyph = null,
                    GlyphColor = "azure",
                    Scale = 1.0,
                    UseIconFonts = true,
                },
                Position = new(gasStationModel.Lat, gasStationModel.Lng),
                Title = gasStationModel.Rotulo,
            });
        }

       await TravelGoogleMap.ShowDataAsync();
    }
}
