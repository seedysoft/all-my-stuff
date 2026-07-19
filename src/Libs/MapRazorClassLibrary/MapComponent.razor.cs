using Microsoft.JSInterop;

namespace Seedysoft.Libs.MapRazorClassLibrary;

/// <summary>
/// Blazor component that manages an interactive map with route visualization and gas station markers.
/// Integrates Leaflet.js for map rendering and provides functionality to display travel routes and nearby gas stations.
/// </summary>
/// <remarks>
/// This component:
/// - Loads a Leaflet map module dynamically on first render
/// - Displays multiple routes with distinct colors
/// - Marks gas stations with color-coded indicators based on fuel prices
/// - Supports cancellation tokens for async operations
/// - Implements IAsyncDisposable for proper resource cleanup
/// 
/// The component uses local JavaScript interop to interact with Leaflet library features.
/// </remarks>
public partial class MapComponent : IAsyncDisposable
{
    /// <summary>
    /// Array of hex color codes used to differentiate multiple routes on the map.
    /// Supports up to 8 distinct routes with varying shades of blue.
    /// </summary>
    private readonly string[] ColorsForRoutes = ["#007FFF", "#0074EA", "#0069D5", "#005EC0", "#0053AB", "#004896", "#003D81", "#00326C"];

    /// <summary>
    /// Initializes the component by creating a DotNet object reference for JavaScript interop.
    /// Called automatically by Blazor during component initialization.
    /// </summary>
    protected override void OnInitialized() => ObjRef = DotNetObjectReference.Create(this);

    /// <summary>
    /// Initializes the map module and creates the map instance on first render.
    /// </summary>
    /// <param name="firstRender">Indicates whether this is the first render pass.</param>
    /// <remarks>
    /// - Skips execution on subsequent renders
    /// - Dynamically imports the Leaflet module from the component's JavaScript folder
    /// - Initializes the map by calling CreateMapAsync()
    /// </remarks>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        if (MapModule == null)
        {
            MapModule = await JsRuntime.InvokeAsync<IJSObjectReference>(
                "import", $"./{Core.Helpers.ContentHelper.ContentPath(typeof(MapComponent))}/js/leafletModule.js");

            await CreateMapAsync();
        }
    }

    /// <summary>
    /// Loads travel routes and gas stations onto the map based on the provided travel query.
    /// </summary>
    /// <param name="model">The travel query containing origin, destination, petroleum products, and max distance.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// Returns null on success, or an error message string if routes cannot be found or an exception occurs.
    /// </returns>
    /// <remarks>
    /// Process flow:
    /// 1. Shows a loading indicator
    /// 2. Clears all existing markers
    /// 3. Retrieves routes from the routing service
    /// 4. Displays routes as colored polylines on the map
    /// 5. Fetches gas stations within the computed bounds
    /// 6. Marks gas stations with color-coded indicators based on fuel prices
    /// 7. Hides the loading indicator
    /// 
    /// Supports cancellation at multiple points during execution.
    /// </remarks>
    public async Task<string?> LoadRoutesAndGasStationsAsync(GasStationPrices.ViewModels.TravelQueryModel model, CancellationToken cancellationToken)
    {
        await ShowLoaderAsync();

        await RemoveAllMarkersAsync();

        IReadOnlyList<(string NombreRuta, double[,] Coordenadas)> res;
        try
        {
            res = await RoutingService.GetRoutesAsync(model.Orig.Location, model.Dest.Location, cancellationToken);
        }
        catch (Exception e)
        {
            return e.ToString();
        }

        if (res.Count == 0)
            return "No routes found";

        await LoadRoutesDataIntoMapAsync(res, cancellationToken);

        await LoadGasStationsIntoMapAsync(model, ComputeBoundsFromRoutes(res, cancellationToken), cancellationToken);

        await HideLoaderAsync();

        return null;

        /// <summary>
        /// Displays route polylines on the map using color-coded lines.
        /// </summary>
        /// <param name="res">Collection of routes with their coordinate arrays.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <remarks>
        /// Each route is assigned a color from the ColorsForRoutes array based on its index.
        /// Respects cancellation requests and breaks early if cancellation is requested.
        /// </remarks>
        async Task LoadRoutesDataIntoMapAsync(
            IReadOnlyList<(string NombreRuta, double[,] Coordenadas)> res
            , CancellationToken cancellationToken)
        {
            for (int i = 0; i < res.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                (string? NombreRuta, double[,]? Coordenadas) = res[i];

                await AddPolylineAsync(arrayPolyline: Coordenadas, color: ColorsForRoutes[i]);
            }
        }

        /// <summary>
        /// Calculates the geographic bounds (bounding box) encompassing all route coordinates.
        /// </summary>
        /// <param name="res">Collection of routes with their coordinate arrays.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A Bounds object representing the geographic area covered by all routes.</returns>
        /// <remarks>
        /// - Uses inverse limits initially to ensure any valid coordinate will adjust the bounds
        /// - Iterates through all coordinates in all routes to find extreme latitude and longitude values
        /// - Returns Empty bounds if cancellation is requested
        /// - Coordinate format: [latitude, longitude] (column 0 = latitude, column 1 = longitude)
        /// </remarks>
        Travel.Models.Bounds ComputeBoundsFromRoutes(
            IReadOnlyList<(string NombreRuta
            , double[,] Coordenadas)> res
            , CancellationToken cancellationToken)
        {
            // Take inverse limits, so any obtained point will be used
            double NorthEastLatitude = Travel.Models.Bounds.Inverse.NorthEast.Latitude;
            double NorthEastLongitude = Travel.Models.Bounds.Inverse.NorthEast.Longitude;
            double SouthWestLatitude = Travel.Models.Bounds.Inverse.SouthWest.Latitude;
            double SouthWestLongitude = Travel.Models.Bounds.Inverse.SouthWest.Longitude;

            foreach ((string NombreRuta, double[,] Coordenadas) in res)
            {
                if (cancellationToken.IsCancellationRequested)
                    return Travel.Models.Bounds.Empty;

                for (int i = 0; i < Coordenadas.GetLength(0); i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return Travel.Models.Bounds.Empty;

                    for (int j = 0; j < Coordenadas.GetLength(1); j++)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return Travel.Models.Bounds.Empty;

                        double v = Coordenadas[i, j];

                        if (j == 0)
                        {
                            // latitude
                            if (v > NorthEastLatitude)
                                NorthEastLatitude = v;
                            if (v < SouthWestLatitude)
                                SouthWestLatitude = v;
                        }
                        else // (j == 1)
                        {
                            // longitude
                            if (v > NorthEastLongitude)
                                NorthEastLongitude = v;
                            if (v < SouthWestLongitude)
                                SouthWestLongitude = v;
                        }
                    }
                }
            }

            Travel.Models.Bounds boundsForGasStations = new(
                NorthEast: new Travel.Models.Location(NorthEastLatitude, NorthEastLongitude),
                SouthWest: new Travel.Models.Location(SouthWestLatitude, SouthWestLongitude));

            return boundsForGasStations;
        }

        /// <summary>
        /// Retrieves gas stations within the calculated bounds and displays them on the map as markers.
        /// </summary>
        /// <param name="model">The travel query model containing selected petroleum product filters.</param>
        /// <param name="bounds">The geographic bounds to search for gas stations.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <remarks>
        /// Process:
        /// 1. Queries the gas station service for stations within the bounds and max distance
        /// 2. Calculates min, average, and max prices for each selected petroleum product
        /// 3. Creates circle markers for each gas station
        /// 4. Color-codes markers to indicate stations with minimum prices (red indicates minimum)
        /// 5. Sets marker size and styling based on price tier (TODO: enhance color and size logic)
        /// 
        /// Future enhancement: Use colors, sizes, and SVG icons to better represent price tiers.
        /// </remarks>
        async Task LoadGasStationsIntoMapAsync(
            GasStationPrices.ViewModels.TravelQueryModel model
            , Travel.Models.Bounds bounds
            , CancellationToken cancellationToken)
        {
            IReadOnlyList<GasStationPrices.ViewModels.GasStationModel> gasStations =
                await GasStationPricesService.GetNearGasStationsAsync(bounds, model.MaxDistanceInKm, cancellationToken);

            // For each product, obtain min and average
            var Products =
                from p in GasStationPrices.Models.Minetur.ProductoPetrolifero.All
                where model.PetroleumProductsSelectedIds.Contains(p.IdProducto)
                let v = gasStations.Select(x => x.GetProdById(p.IdProducto)).Where(x => x.HasValue)
                select new
                {
                    IdP = p.IdProducto,
                    Min = v.Min(),
                    Avg = v.Average(),
                    Max = v.Max(),
                };

            for (int i = 0; i < gasStations.Count; i++)
            {
                GasStationPrices.ViewModels.GasStationModel GasStation = gasStations[i];

                IReadOnlyList<(GasStationPrices.Constants.ProductoPetroliferoId IdProducto, decimal Value)> GasStationProducts =
                    GasStation.AllProducts(model.PetroleumProductsSelectedIds);

                //                          TODO Use colors, sizes, etc...
                MapModels.VectorLayers.CircleMarker circleMarker = new(new MapModels.Basic.LatLng(GasStation.Lat, GasStation.Lon))
                {
                    ClassName = "material-icons local_gas_station",
                    Fill = true,
                    FillOpacity = 1.0,
                    FillRule = "nonzero",
                };

                if (GasStationProducts.Any(x => x.Value == (Products.FirstOrDefault(p => p.IdP == x.IdProducto)?.Min ?? decimal.Zero)))
                {
                    circleMarker.Color = circleMarker.FillColor = "#ff0000"; // Green = Min
                    circleMarker.Radius = 5;
                }
                else if (GasStationProducts.Any(x => x.Value == (Products.FirstOrDefault(p => p.IdP == x.IdProducto)?.Max ?? decimal.Zero)))
                {
                    circleMarker.Color = circleMarker.FillColor = "#00ff00"; // Red = Max
                    circleMarker.Radius = 20;
                }
                else if (GasStationProducts.Any(x => x.Value <= (Products.FirstOrDefault(p => p.IdP == x.IdProducto)?.Avg ?? decimal.Zero)))
                {
                    circleMarker.Color = circleMarker.FillColor = "#FFFF00"; // Yellow <= Avg
                    circleMarker.Radius = 10;
                }
                else
                {
                    circleMarker.Color = circleMarker.FillColor = "#FF8C00"; // Orange > Avg
                    circleMarker.Radius = 15;
                }

                MapModels.UILayers.Popup popup = new()
                {
                    Content = $"<b>{GasStation.Localizacion}</b>",
                };

                MapModels.UILayers.Tooltip tooltip = new()
                {
                    Content = $"<b>{GasStation.RotuloTrimed}</b>",
                    Direction = MapModels.UILayers.Tooltip.Directions.Top,
                    Permanent = true,
                };

                await AddCircleMarker(circleMarker, popup, tooltip);
            }
        }
    }
}
