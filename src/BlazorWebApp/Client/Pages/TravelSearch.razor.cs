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

    private readonly Libs.GasStationPrices.Core.ViewModels.TravelQueryModelFluentValidator travelQueryModelFluentValidator = new();

    private MudBlazor.MudForm TravelQueryMudForm { get; set; } = default!;
    private GoogleMapsComponents.GoogleMap TravelGoogleMap { get; set; } = default!;

    private GoogleMapsComponents.Maps.MapOptions mapOptions = default!;
    private GoogleMapsComponents.Maps.DirectionsRenderer directionsRenderer = default!;
    private GoogleMapsComponents.Maps.Extension.AdvancedMarkerElementList? advancedMarkerElementList;

    private readonly Libs.GasStationPrices.Core.ViewModels.TravelQueryModel travelQueryModel = new()
    {
#if DEBUG
        Origin = "Calle Juan Ramon Jimenez, 8, Burgos, Spain",
        Destination = "Calle la Iglesia, 11, Brazuelo, Leon, Spain",
#else
        Origin = string.Empty,
        Destination = string.Empty,
#endif
        MaxDistanceInKm = 5M,
        PetroleumProductsSelectedIds = [],
        Bounds = default!,
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

        travelQueryModel.PetroleumProductsSelectedIds =
            PetroleumProducts.Where(static x => x.Abreviatura.StartsWith("G9")).Select(static x => x.IdProducto).ToArray().AsReadOnly();

        mapOptions = new()
        {
            Center = new GoogleMapsComponents.Maps.LatLngLiteral() { Lat = -3.6659289163890474, Lng = 42.35378137832184, },
            MapId = "cbf4875eb632c9db",
            ZoomControl = true,
            //ApiLoadOptions = new() { Language = "en-US", },
            //CameraControl = false,
            //ClickableIcons = false,
            //DisableDefaultUI = true,
            //Draggable = false,
            //FullscreenControl = false,
            //HeadingInteractionEnabled = false,
            //IsFractionalZoomEnabled = false,
            //KeyboardShortcuts = false,
            //MapTypeControl = false,
            //MapTypeId = GoogleMapsComponents.Maps.MapTypeId.Satellite,
            //NoClear = true,
            //PanControl = false,
            ////RenderingType = GoogleMapsComponents.Maps.RenderingType.Uninitialized,
            ////Restriction = new() { latLngBounds = new() { }, strictBounds = false, },
            //RotateControl = false,
            //ScaleControl = false,
            //Scrollwheel = false,
            //StreetViewControl = false,
            //Zoom = 13,
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

    private void SelectAllChips() => travelQueryModel.PetroleumProductsSelectedIds = PetroleumProducts.Select(static x => x.IdProducto).ToArray().AsReadOnly();
    private void UnSelectAllChips() => travelQueryModel.PetroleumProductsSelectedIds = [];

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

        //Adding a waypoint
        List<GoogleMapsComponents.Maps.DirectionsWaypoint> directionsWaypoints =
        [
        //new GoogleMapsComponents.Maps.DirectionsWaypoint() { Location = "Bethlehem, PA", Stopover = true }
        ];

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
            Waypoints = directionsWaypoints,
        };

        //Calculate Route
        GoogleMapsComponents.Maps.DirectionsResult? directionsResult = await directionsRenderer.Route(directionsRequest);
        if (directionsResult == null)
            return;

        travelQueryModel.Bounds.North = directionsResult.Routes.Select(static x => x.Bounds?.North ?? 90D).Max();
        travelQueryModel.Bounds.South = directionsResult.Routes.Select(static x => x.Bounds?.South ?? -90D).Min();
        travelQueryModel.Bounds.East = directionsResult.Routes.Select(static x => x.Bounds?.East ?? 180D).Max();
        travelQueryModel.Bounds.West = directionsResult.Routes.Select(static x => x.Bounds?.West ?? -180D).Min();

        // TODO         Use directionsResult

        StringContent requestContent = new(
            System.Text.Json.JsonSerializer.Serialize(travelQueryModel),
            System.Text.Encoding.UTF8,
            System.Net.Mime.MediaTypeNames.Application.Json);
        string FromUri = $"{NavManager.BaseUri}{Constants.TravelUris.Controller}/{Constants.TravelUris.Actions.GetGasStations}";
        HttpRequestMessage requestMessage = new(HttpMethod.Post, FromUri) { Content = requestContent };
#pragma warning disable CA1416 // Validate platform compatibility
        HttpResponseMessage response = Http.Send(requestMessage);
#pragma warning restore CA1416 // Validate platform compatibility

        if (!response.IsSuccessStatusCode)
            return;

        advancedMarkerElementList ??= await GoogleMapsComponents.Maps.Extension.AdvancedMarkerElementList.CreateAsync(TravelGoogleMap.JsRuntime, []);

        await foreach (Libs.GasStationPrices.Core.ViewModels.GasStationModel? gasStationModel in response.Content.ReadFromJsonAsAsyncEnumerable<Libs.GasStationPrices.Core.ViewModels.GasStationModel>()!)
        {
            if (gasStationModel == null)
                continue;

            const string Svg = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"56\" height=\"56\" viewBox=\"0 0 56 56\" fill=\"none\">\r\n  <rect width=\"56\" height=\"56\" rx=\"28\" fill=\"#7837FF\"></rect>\r\n  <path d=\"M46.0675 22.1319L44.0601 22.7843\" stroke=\"white\" stroke-width=\"2.5\" stroke-linecap=\"round\" stroke-linejoin=\"round\"></path>\r\n  <path d=\"M11.9402 33.2201L9.93262 33.8723\" stroke=\"white\" stroke-width=\"2.5\" stroke-linecap=\"round\" stroke-linejoin=\"round\"></path>\r\n  <path d=\"M27.9999 47.0046V44.8933\" stroke=\"white\" stroke-width=\"2.5\" stroke-linecap=\"round\" stroke-linejoin=\"round\"></path>\r\n  <path d=\"M27.9999 9V11.1113\" stroke=\"white\" stroke-width=\"2.5\" stroke-linecap=\"round\" stroke-linejoin=\"round\"></path>\r\n  <path d=\"M39.1583 43.3597L37.9186 41.6532\" stroke=\"white\" stroke-width=\"2.5\" stroke-linecap=\"round\" stroke-linejoin=\"round\"></path>\r\n  <path d=\"M16.8419 12.6442L18.0816 14.3506\" stroke=\"white\" stroke-width=\"2.5\" stroke-linecap=\"round\" stroke-linejoin=\"round\"></path>\r\n  <path d=\"M9.93262 22.1319L11.9402 22.7843\" stroke=\"white\" stroke-width=\"2.5\" stroke-linecap=\"round\" stroke-linejoin=\"round\"></path>\r\n  <path d=\"M46.0676 33.8724L44.0601 33.2201\" stroke=\"white\" stroke-width=\"2.5\" stroke-linecap=\"round\" stroke-linejoin=\"round\"></path>\r\n  <path d=\"M39.1583 12.6442L37.9186 14.3506\" stroke=\"white\" stroke-width=\"2.5\" stroke-linecap=\"round\" stroke-linejoin=\"round\"></path>\r\n  <path d=\"M16.8419 43.3597L18.0816 41.6532\" stroke=\"white\" stroke-width=\"2.5\" stroke-linecap=\"round\" stroke-linejoin=\"round\"></path>\r\n  <path d=\"M28 39L26.8725 37.9904C24.9292 36.226 23.325 34.7026 22.06 33.4202C20.795 32.1378 19.7867 30.9918 19.035 29.9823C18.2833 28.9727 17.7562 28.0587 17.4537 27.2401C17.1512 26.4216 17 25.5939 17 24.7572C17 23.1201 17.5546 21.7513 18.6638 20.6508C19.7729 19.5502 21.1433 19 22.775 19C23.82 19 24.7871 19.2456 25.6762 19.7367C26.5654 20.2278 27.34 20.9372 28 21.8649C28.77 20.8827 29.5858 20.1596 30.4475 19.6958C31.3092 19.2319 32.235 19 33.225 19C34.8567 19 36.2271 19.5502 37.3362 20.6508C38.4454 21.7513 39 23.1201 39 24.7572C39 25.5939 38.8488 26.4216 38.5463 27.2401C38.2438 28.0587 37.7167 28.9727 36.965 29.9823C36.2133 30.9918 35.205 32.1378 33.94 33.4202C32.675 34.7026 31.0708 36.226 29.1275 37.9904L28 39Z\" fill=\"#FF7878\"></path>\r\n</svg>";

            GoogleMapsComponents.Maps.AdvancedMarkerElementOptions marker = new()
            {
                CollisionBehavior = GoogleMapsComponents.Maps.CollisionBehavior.REQUIRED,
                //Content = $"<div style='background-color:blue'>{gasStationModel.Rotulo}</div>",
                //Content = new GoogleMapsComponents.Maps.PinElement() { },
                Content = Svg,
                GmpClickable = true,
                GmpDraggable = false,
                Map = TravelGoogleMap.InteropObject,
                Position = gasStationModel.LatLon,
                Title = gasStationModel.Rotulo,
                ZIndex = 0,
            };

            var opts = new Dictionary<string, GoogleMapsComponents.Maps.AdvancedMarkerElementOptions>() { { Guid.NewGuid().ToString("N"), marker }, };

            await advancedMarkerElementList.AddMultipleAsync(opts);
        }
    }

    private async Task RemoveAllMarkersAsync()
    {
        if (advancedMarkerElementList == null)
            return;

        foreach (KeyValuePair<string, GoogleMapsComponents.Maps.AdvancedMarkerElement> markerListMarker in advancedMarkerElementList.Markers)
            await markerListMarker.Value.SetMap(null);

        await advancedMarkerElementList.RemoveAllAsync();
    }
}
