using Microsoft.AspNetCore.Components;
using Seedysoft.Libs.Utils.Extensions;
using System.Net.Http.Json;

namespace Seedysoft.BlazorWebApp.Client.Pages;

public partial class TravelSearch
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private ILogger<TravelSearch> Logger { get; set; } = default!;
    [Inject] private NavigationManager NavManager { get; set; } = default!;
    [Inject] private MudBlazor.ISnackbar Snackbar { get; set; } = default!;

    private MudBlazor.MudForm TravelQueryMudForm { get; set; } = default!;

    private GoogleMapsComponents.GoogleMap TravelGoogleMap { get; set; } = default!;
    private GoogleMapsComponents.Maps.MapOptions mapOptions = default!;
    private GoogleMapsComponents.Maps.DirectionsRenderer directionsRenderer = default!;
    private readonly Stack<GoogleMapsComponents.Maps.AdvancedMarkerElement> advancedMarkerElements = new();

    private readonly Libs.GasStationPrices.Core.ViewModels.TravelQueryModelFluentValidator travelQueryModelFluentValidator = new();
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

    private IEnumerable<Libs.GasStationPrices.Core.Json.Minetur.ProductoPetrolifero> PetroleumProducts = [];

    protected override async Task OnInitializedAsync()
    {
        Logger.LogInformation($"Called {nameof(OnInitializedAsync)}");

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

        FromUri = $"{NavManager.BaseUri}{Constants.TravelUris.Controller}/{Constants.TravelUris.Actions.GetMapId}";
        string MapId = await Http.GetStringAsync(FromUri);

        mapOptions = new()
        {
            //ApiLoadOptions = new() { Language = "en-US", },
            //BackgroundColor = "",
            //CameraControl = false,
            //CameraControlOptions = new() { Position = GoogleMapsComponents.Maps.ControlPosition.TopLeft, },
            Center = new GoogleMapsComponents.Maps.LatLngLiteral()
            {
                Lat = Libs.Core.Constants.Earth.Home.Lat,
                Lng = Libs.Core.Constants.Earth.Home.Lng,
            },
            //ClickableIcons = false,
            ColorScheme = GoogleMapsComponents.Maps.ColorScheme.FollowSystem,
            //ControlSize = 10,
            //DisableDefaultUI = true,
            //DisableDoubleClickZoom = false,
            //Draggable = false,
            //DraggableCursor = "",
            //DraggingCursor = "",
            //FullscreenControl = false,
            //FullscreenControlOptions = new() { Position = GoogleMapsComponents.Maps.ControlPosition.TopCenter, },
            //Heading = 5,
            //HeadingInteractionEnabled = false,
            //IsFractionalZoomEnabled = false,
            //KeyboardShortcuts = false,
            MapId = MapId,
            //MapTypeControl = false,
            //MapTypeControlOptions = new()
            //{
            //    MapTypeIds = [GoogleMapsComponents.Maps.MapTypeId.Hybrid, GoogleMapsComponents.Maps.MapTypeId.Terrain,],
            //    Position = GoogleMapsComponents.Maps.ControlPosition.TopRight,
            //    Style = GoogleMapsComponents.Maps.MapTypeControlStyle.Default,
            //},
            MapTypeId = GoogleMapsComponents.Maps.MapTypeId.Roadmap,
            //MaxZoom = 30,
            //MinZoom = 0,
            //NoClear = true,
            //PanControl = false,
            //PanControlOptions = new() { Position = GoogleMapsComponents.Maps.ControlPosition.LeftTop, },
            ////RenderingType = GoogleMapsComponents.Maps.RenderingType.Uninitialized,
            ////Restriction = new() { latLngBounds = new() { }, strictBounds = false, },
            //RotateControl = false,
            //RotateControlOptions = new() { Position = GoogleMapsComponents.Maps.ControlPosition.LeftCenter, },
            //ScaleControl = false,
            //ScaleControlOptions = new() { Style = GoogleMapsComponents.Maps.ScaleControlStyle.Default, },
            //Scrollwheel = false,
            StreetViewControl = false,
            //StreetViewControlOptions = new() { Position = GoogleMapsComponents.Maps.ControlPosition.BottomCenter, },
            //Styles = [new() { elementType = "", featureType = "", stylers = [new GoogleMapsComponents.Maps.GoogleMapStyleColor() { color = "", }] }],
            //Tilt = 10,
            Zoom = 14,
            ZoomControl = true,
            //ZoomControlOptions = new() { Position = GoogleMapsComponents.Maps.ControlPosition.BottomRight, },
        };
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

    private async Task GoogleMapOnAfterInitAsync()
    {
        GoogleMapsComponents.Maps.LatLngBounds _bounds = await GoogleMapsComponents.Maps.LatLngBounds.CreateAsync(TravelGoogleMap.JsRuntime);
        Console.WriteLine($"Bounds: {_bounds.ToJson()}");

        //Create instance of DirectionRenderer
        directionsRenderer = await GoogleMapsComponents.Maps.DirectionsRenderer.CreateAsync(
            TravelGoogleMap!.JsRuntime,
            new() { Map = TravelGoogleMap.InteropObject }
        );
    }

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

        await RemoveAllMarkersAsync();

        if (await directionsRenderer.GetMap() is null)
            await directionsRenderer.SetMap(TravelGoogleMap!.InteropObject);

        //Direction Request
        GoogleMapsComponents.Maps.DirectionsRequest directionsRequest = new()
        {
            Origin = travelQueryModel.Origin,
            Destination = travelQueryModel.Destination,
            AvoidFerries = false,
            AvoidHighways = false,
            AvoidTolls = false,
            DrivingOptions = new() { DepartureTime = DateTime.UtcNow, TrafficModel = GoogleMapsComponents.Maps.TrafficModel.bestguess, },
            OptimizeWaypoints = false,
            ProvideRouteAlternatives = true,
            //Region = "es",
            //TransitOptions = new()
            //{
            //    ArrivalTime = DateTime.UtcNow,
            //    DepartureTime = DateTime.UtcNow,
            //    Modes = [GoogleMapsComponents.Maps.TransitMode.Bus, GoogleMapsComponents.Maps.TransitMode.Train],
            //    RoutingPreference = GoogleMapsComponents.Maps.TransitRoutePreference.FewerTransfers,
            //},
            TravelMode = GoogleMapsComponents.Maps.TravelMode.Driving,
            //UnitSystem = GoogleMapsComponents.Maps.UnitSystem.Metric, // InvalidValueError: in property unitSystem: metric is not an accepted value
            Waypoints = [
                //new GoogleMapsComponents.Maps.DirectionsWaypoint() { Location = "Bethlehem, PA", Stopover = true }
            ],
        };

        //Calculate Route
        GoogleMapsComponents.Maps.DirectionsResult? directionsResult = await directionsRenderer.Route(directionsRequest);
        if (directionsResult == null)
            return;

        travelQueryModel.Bounds.North = directionsResult.Routes.Select(static x => x.Bounds?.North ?? 90D).Max();
        travelQueryModel.Bounds.South = directionsResult.Routes.Select(static x => x.Bounds?.South ?? -90D).Min();
        travelQueryModel.Bounds.East = directionsResult.Routes.Select(static x => x.Bounds?.East ?? 180D).Max();
        travelQueryModel.Bounds.West = directionsResult.Routes.Select(static x => x.Bounds?.West ?? -180D).Min();

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

            GoogleMapsComponents.Maps.AdvancedMarkerElementOptions advancedMarkerElementOptions = new()
            {
                CollisionBehavior = GoogleMapsComponents.Maps.CollisionBehavior.REQUIRED,
                Content = $"<div style='background-color:blue'>{gasStationModel.Rotulo}</div>",
                //Content = new GoogleMapsComponents.Maps.PinElement() { },
                GmpClickable = true,
                GmpDraggable = false,
                Map = TravelGoogleMap.InteropObject,
                Position = gasStationModel.LatLon,
                Title = gasStationModel.Rotulo,
                ZIndex = 0,
            };

            GoogleMapsComponents.Maps.AdvancedMarkerElement advancedMarkerElement =
                await GoogleMapsComponents.Maps.AdvancedMarkerElement.CreateAsync(TravelGoogleMap!.JsRuntime, advancedMarkerElementOptions);

            _ = advancedMarkerElements.Append(advancedMarkerElement);
        }
    }

    private async Task RemoveAllMarkersAsync()
    {
        if (advancedMarkerElements == null)
            return;

        foreach (GoogleMapsComponents.Maps.AdvancedMarkerElement markerListMarker in advancedMarkerElements)
            await markerListMarker.SetMap(null);

        advancedMarkerElements.Clear();
    }
}
