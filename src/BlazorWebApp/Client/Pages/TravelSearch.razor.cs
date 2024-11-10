using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Seedysoft.Libs.Utils.Extensions;
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

    private Libs.GasStationPrices.Core.Settings.SettingsRoot Settings;

    private MudBlazor.MudForm TravelQueryMudForm { get; set; } = default!;
    private Libs.GoogleMapsRazorClassLib.GoogleMap.Map TravelGoogleMap { get; set; } = default!;

    private Libs.GoogleMapsRazorClassLib.Directions.Service directionsService = default!;
    private readonly List<Libs.GoogleMapsRazorClassLib.GoogleMap.Marker> markers = [];

    private readonly Libs.GasStationPrices.Core.ViewModels.TravelQueryModel travelQueryModel = new()
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
    private readonly Libs.GasStationPrices.Core.ViewModels.TravelQueryModelFluentValidator travelQueryModelFluentValidator = new();

    private IEnumerable<Libs.GasStationPrices.Core.Json.Minetur.ProductoPetrolifero> PetroleumProducts = [];

    protected override async Task OnInitializedAsync()
    {
        Logger.LogInformation($"Called {nameof(OnInitializedAsync)}");

        Settings =
            Configuration.GetSection(nameof(Libs.GasStationPrices.Core.Settings.SettingsRoot)).Get<Libs.GasStationPrices.Core.Settings.SettingsRoot>()!;

        await base.OnInitializedAsync();

        // TODO         Take only from Libs.GasStationPrices.Core.Json.Minetur.ProductoPetrolifero

        string FromUri = $"{NavManager.BaseUri}{Constants.PetroleumProductsUris.Controller}/{Constants.PetroleumProductsUris.Actions.ForFilter}";
        PetroleumProducts = await Http.GetFromJsonAsync<IEnumerable<Libs.GasStationPrices.Core.Json.Minetur.ProductoPetrolifero>>(FromUri) ?? [];

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
            // TODO             CACHE
            // TODO Validation
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

    private async Task LoadDataAsync()
    {
        FluentValidation.Results.ValidationResult validationResult = await travelQueryModelFluentValidator.ValidateAsync(travelQueryModel);
        if (!validationResult.IsValid)
        {
            IEnumerable<string> errors = validationResult.Errors.Select(static x => $"<li>{x.ErrorMessage}</li>");
            _ = Snackbar.Add(new MarkupString($"<ul>{string.Join("", errors)}</ul>"), MudBlazor.Severity.Error);
            return;
        }

        RemoveAllMarkers();

        // TODO          Make Request and remove Bounds dependency

        //Direction Request
        Libs.GoogleMapsRazorClassLib.Directions.Request directionsRequest = new()
        {
            Origin = travelQueryModel.Origin,
            Destination = travelQueryModel.Destination,
            TravelMode = Libs.GoogleMapsRazorClassLib.Directions.TravelMode.Driving,
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
            ////UnitSystem = GoogleMapsComponents.Maps.UnitSystem.Metric, // InvalidValueError: in property unitSystem: metric is not an accepted value
            //Waypoints = [
            //    //new GoogleMapsComponents.Maps.DirectionsWaypoint() { Location = "Bethlehem, PA", Stopover = true }
            //],
        };
        directionsService ??= new(JSRuntime, TravelGoogleMap.Id);
        //Calculate Route
        Libs.GoogleMapsRazorClassLib.Directions.Leg[]? directionsLegs = await directionsService.Route(directionsRequest);
        if (directionsLegs == null)
            return;

        IEnumerable<Libs.GoogleMapsRazorClassLib.Directions.LatLngLiteral> LatLngsQuery =
            directionsLegs.SelectMany(x => x.Steps).SelectMany(x => x.LatLngs);

        travelQueryModel.Bounds.North = LatLngsQuery.Max(x => x?.Lat) ?? Libs.Core.Constants.Earth.MaxLatitudeInDegrees;
        travelQueryModel.Bounds.South = LatLngsQuery.Min(x => x?.Lat) ?? Libs.Core.Constants.Earth.MinLatitudeInDegrees;
        travelQueryModel.Bounds.East = LatLngsQuery.Max(x => x?.Lng) ?? Libs.Core.Constants.Earth.MaxLongitudeInDegrees;
        travelQueryModel.Bounds.West = LatLngsQuery.Min(x => x?.Lng) ?? Libs.Core.Constants.Earth.MinLongitudeInDegrees;

        StringContent requestContent = new(
            System.Text.Json.JsonSerializer.Serialize(travelQueryModel),
            System.Text.Encoding.UTF8,
            System.Net.Mime.MediaTypeNames.Application.Json);
        string FromUri = $"{NavManager.BaseUri}{Constants.TravelUris.Controller}/{Constants.TravelUris.Actions.GetGasStations}";
        HttpRequestMessage requestMessage = new(HttpMethod.Post, FromUri) { Content = requestContent, };
#pragma warning disable CA1416 // Validate platform compatibility
        HttpResponseMessage response = Http.Send(requestMessage);
#pragma warning restore CA1416 // Validate platform compatibility

        if (!response.IsSuccessStatusCode)
            return;

        await foreach (Libs.GasStationPrices.Core.ViewModels.GasStationModel? gasStationModel in
            response.Content.ReadFromJsonAsAsyncEnumerable<Libs.GasStationPrices.Core.ViewModels.GasStationModel>()!)
        {
            if (gasStationModel == null)
                continue;

            Libs.GoogleMapsRazorClassLib.GoogleMap.Marker marker = new()
            {
                Content = $"<div style='background-color:blue'>{gasStationModel.Rotulo}</div>",
                PinElement = new()
                {
                    Background = "",
                    BorderColor = "",
                    Glyph = null,
                    GlyphColor = "",
                    Scale = 1.0,
                    UseIconFonts = true,
                },
                Position = new(gasStationModel.Lat, gasStationModel.Lng),
                Title = gasStationModel.Rotulo,
            };

            _ = markers.Append(marker);
        }
    }

    private void RemoveAllMarkers() => markers?.Clear();
}
