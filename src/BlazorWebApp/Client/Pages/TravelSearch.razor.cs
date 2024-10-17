using GoogleMapsLibrary.Maps;
using GoogleMapsLibrary.Maps.Coordinates;
using GoogleMapsLibrary.Marker;
using GoogleMapsLibrary.Routes;
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

    private GoogleMapsJavascriptApi.GoogleMapComponent TravelGoogleMap { get; set; } = default!;
    private MapOptions mapOptions = default!;
    private DirectionsRenderer directionsRenderer = default!;
    private readonly Stack<AdvancedMarkerElement> advancedMarkerElements = new();

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
            //BackgroundColor = string.Empty,
            //CameraControl = false,
            //CameraControlOptions = new() { Position = ControlPosition.TopLeft, },
            Center = new LatLngLiteral()
            {
                Lat = Libs.Core.Constants.Earth.Home.Lat,
                Lng = Libs.Core.Constants.Earth.Home.Lng,
            },
            //ClickableIcons = false,
            ColorScheme = GoogleMapsLibrary.Core.ColorScheme.FollowSystem,
            //ControlSize = 10,
            //DisableDefaultUI = true,
            //DisableDoubleClickZoom = false,
            //Draggable = false,
            //DraggableCursor = string.Empty,
            //DraggingCursor = string.Empty,
            //FullscreenControl = false,
            //FullscreenControlOptions = new() { Position = ControlPosition.TopCenter, },
            //Heading = 5,
            //HeadingInteractionEnabled = false,
            //IsFractionalZoomEnabled = false,
            //KeyboardShortcuts = false,
            MapId = MapId,
            //MapTypeControl = false,
            //MapTypeControlOptions = new()
            //{
            //    MapTypeIds = [MapTypeId.Hybrid, MapTypeId.Terrain,],
            //    Position = ControlPosition.TopRight,
            //    Style = MapTypeControlStyle.Default,
            //},
            MapTypeId = MapTypeId.Roadmap,
            //MaxZoom = 30,
            //MinZoom = 0,
            //NoClear = true,
            //PanControl = false,
            //PanControlOptions = new() { Position = ControlPosition.LeftTop, },
            ////RenderingType = RenderingType.Uninitialized,
            ////Restriction = new() { latLngBounds = new() { }, strictBounds = false, },
            //RotateControl = false,
            //RotateControlOptions = new() { Position = ControlPosition.LeftCenter, },
            //ScaleControl = false,
            //ScaleControlOptions = new() { Style = ScaleControlStyle.Default, },
            //Scrollwheel = false,
            StreetViewControl = false,
            //StreetViewControlOptions = new() { Position = ControlPosition.BottomCenter, },
            //Styles = [new() { elementType = "", featureType = "", stylers = [new GoogleMapStyleColor() { color = "", }] }],
            //Tilt = 10,
            Zoom = 14,
            ZoomControl = true,
            //ZoomControlOptions = new() { Position = ControlPosition.BottomRight, },
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
        LatLngBounds _bounds = await LatLngBounds.CreateAsync(TravelGoogleMap.JsRuntime);
        Console.WriteLine($"Bounds: {_bounds.ToJson()}");

        //Create instance of DirectionRenderer
        directionsRenderer = await DirectionsRenderer.CreateAsync(
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
        //        FluentValidation.Results.ValidationResult validationResult = await travelQueryModelFluentValidator.ValidateAsync(travelQueryModel);
        //        if (!validationResult.IsValid)
        //        {
        //            IEnumerable<string> errors = validationResult.Errors.Select(static x => $"<li>{x.ErrorMessage}</li>");
        //            _ = Snackbar.Add(new MarkupString($"<ul>{string.Join(string.Empty, errors)}</ul>"), MudBlazor.Severity.Error);
        //            return;
        //        }

        //        await RemoveAllMarkersAsync();

        //        if (await directionsRenderer.GetMap() is null)
        //            await directionsRenderer.SetMap(TravelGoogleMap!.InteropObject);

        //        //Direction Request
        //        DirectionsRequest directionsRequest = new()
        //        {
        //            Origin = travelQueryModel.Origin,
        //            Destination = travelQueryModel.Destination,
        //            AvoidFerries = false,
        //            AvoidHighways = false,
        //            AvoidTolls = false,
        //            DrivingOptions = new() { DepartureTime = DateTime.UtcNow, TrafficModel = TrafficModel.BestGuess, },
        //            OptimizeWaypoints = false,
        //            ProvideRouteAlternatives = true,
        //            //Region = "es",
        //            //TransitOptions = new()
        //            //{
        //            //    ArrivalTime = DateTime.UtcNow,
        //            //    DepartureTime = DateTime.UtcNow,
        //            //    Modes = [TransitMode.Bus, TransitMode.Train],
        //            //    RoutingPreference = TransitRoutePreference.FewerTransfers,
        //            //},
        //            TravelMode = TravelMode.Driving,
        //            //UnitSystem = UnitSystem.Metric, // InvalidValueError: in property unitSystem: metric is not an accepted value
        //            Waypoints = [
        //                //new DirectionsWaypoint() { Location = "Bethlehem, PA", Stopover = true }
        //            ],
        //        };

        //        //Calculate Route
        //        DirectionsResult? directionsResult = await directionsRenderer.Route(directionsRequest);
        //        if (directionsResult == null)
        //            return;

        //        travelQueryModel.Bounds.North =
        //            directionsResult.Routes.Select(static x => x.Bounds?.North ?? Libs.Core.Constants.Earth.MaxLatitudeInDegrees).Max();
        //        travelQueryModel.Bounds.South =
        //            directionsResult.Routes.Select(static x => x.Bounds?.South ?? Libs.Core.Constants.Earth.MinLatitudeInDegrees).Min();
        //        travelQueryModel.Bounds.East =
        //            directionsResult.Routes.Select(static x => x.Bounds?.East ?? Libs.Core.Constants.Earth.MaxLongitudeInDegrees).Max();
        //        travelQueryModel.Bounds.West =
        //            directionsResult.Routes.Select(static x => x.Bounds?.West ?? Libs.Core.Constants.Earth.MinLongitudeInDegrees).Min();

        //        StringContent requestContent = new(
        //            System.Text.Json.JsonSerializer.Serialize(travelQueryModel),
        //            System.Text.Encoding.UTF8,
        //            System.Net.Mime.MediaTypeNames.Application.Json);
        //        string FromUri = $"{NavManager.BaseUri}{Constants.TravelUris.Controller}/{Constants.TravelUris.Actions.GetGasStations}";
        //        HttpRequestMessage requestMessage = new(HttpMethod.Post, FromUri) { Content = requestContent, };
        //#pragma warning disable CA1416 // Validate platform compatibility
        //        HttpResponseMessage response = Http.Send(requestMessage);
        //#pragma warning restore CA1416 // Validate platform compatibility

        //        if (!response.IsSuccessStatusCode)
        //            return;

        //        await foreach (Libs.GasStationPrices.Core.ViewModels.GasStationModel? gasStationModel in
        //            response.Content.ReadFromJsonAsAsyncEnumerable<Libs.GasStationPrices.Core.ViewModels.GasStationModel>()!)
        //        {
        //            if (gasStationModel == null)
        //                continue;

        //            AdvancedMarkerElementOptions advancedMarkerElementOptions = new()
        //            {
        //                CollisionBehavior = CollisionBehavior.Required,
        //                Content = $"<div style='background-color:blue'>{gasStationModel.Rotulo}</div>",
        //                //Content = new PinElement() { },
        //                GmpClickable = true,
        //                GmpDraggable = false,
        //                Map = TravelGoogleMap.InteropObject,
        //                Position = gasStationModel.LatLon,
        //                Title = gasStationModel.Rotulo,
        //                ZIndex = 0,
        //            };

        //            AdvancedMarkerElement advancedMarkerElement =
        //                await AdvancedMarkerElement.CreateAsync(TravelGoogleMap!.JsRuntime, advancedMarkerElementOptions);

        //            _ = advancedMarkerElements.Append(advancedMarkerElement);
        //        }
    }

    private async Task RemoveAllMarkersAsync()
    {
        //if (advancedMarkerElements == null)
        //    return;

        //foreach (AdvancedMarkerElement markerListMarker in advancedMarkerElements)
        //    await markerListMarker.SetMap(null);

        //advancedMarkerElements.Clear();
    }
}
