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
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
            return;

        if (MapModule == null)
        {
            MapModule = await JsRuntime.InvokeAsync<IJSObjectReference>(
                "import", $"./{Assets["_content/Seedysoft.Libs.MapRazorClassLibrary/js/leafletModule.js"]}");

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

        string? LoadRoutesDataIntoMapAsyncResult = await LoadRoutesDataIntoMapAsync(res, cancellationToken);
        if (!string.IsNullOrWhiteSpace(LoadRoutesDataIntoMapAsyncResult))
            return LoadRoutesDataIntoMapAsyncResult;

        string? LoadGasStationsIntoMapAsyncResult = await LoadGasStationsIntoMapAsync(model, ComputeBoundsFromRoutes(res, cancellationToken), cancellationToken);
        if (!string.IsNullOrWhiteSpace(LoadGasStationsIntoMapAsyncResult))
            return LoadGasStationsIntoMapAsyncResult;

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
        async Task<string?> LoadRoutesDataIntoMapAsync(
            IReadOnlyList<(string NombreRuta, double[,] Coordenadas)> res
            , CancellationToken cancellationToken)
        {
            if (res.Count == 0)
                return "⚠️ No route to load ⚠️";

            for (int i = 0; i < res.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                (string? NombreRuta, double[,]? Coordenadas) = res[i];

                await AddPolylineAsync(arrayPolyline: Coordenadas, color: ColorsForRoutes[i]);
            }

            return null;
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
        async Task<string?> LoadGasStationsIntoMapAsync(
            GasStationPrices.ViewModels.TravelQueryModel model
            , Travel.Models.Bounds bounds
            , CancellationToken cancellationToken)
        {
            IReadOnlyList<GasStationPrices.ViewModels.GasStationModel> gasStations =
                await GasStationPricesService.GetNearGasStationsAsync(bounds, model.MaxDistanceInKm, cancellationToken);
            if (gasStations.Count == 0)
                return "⚠️ No Gas stations loaded ⚠️";

            // For each product, obtain min and average
            IEnumerable<ProductLimits> Products =
                from p in GasStationPrices.Models.Minetur.ProductoPetrolifero.All
                where model.PetroleumProductsSelectedIds.Contains(p.IdProducto)
                let v = gasStations.Select(x => x.GetProdById(p.IdProducto)).Where(x => x.HasValue)
                select new ProductLimits(
                    p.IdProducto,
                    v.Min(),
                    v.Average(),
                    v.Max()
                );
            if ((Products.TryGetNonEnumeratedCount(out int count) && count == 0) || !Products.Any())
                return "⚠️ No Products to show ⚠️";

            for (int i = 0; i < gasStations.Count; i++)
            {
                GasStationPrices.ViewModels.GasStationModel GasStation = gasStations[i];

                IReadOnlyList<(GasStationPrices.Constants.ProductoPetroliferoId IdProducto, decimal Value)> GasStationProducts =
                    GasStation.AllProducts(model.PetroleumProductsSelectedIds);

                //                          TODO Use colors, sizes, etc...
                MapModels.VectorLayers.CircleMarker circleMarker = new(new MapModels.Basic.LatLng(GasStation.Lat, GasStation.Lon))
                {
                    Fill = true,
                    FillOpacity = 1.0,
                    FillRule = "nonzero",
                };

                string CssClass;
                if (GasStationProducts.Any(x => x.Value == (Products.FirstOrDefault(p => p.IdP == x.IdProducto)?.Min ?? decimal.Zero)))
                {
                    CssClass = "bgVerde";
                    circleMarker.Color = circleMarker.FillColor = "#ff0000"; // Green = Min
                    circleMarker.Radius = 5;
                }
                else if (GasStationProducts.Any(x => x.Value == (Products.FirstOrDefault(p => p.IdP == x.IdProducto)?.Max ?? decimal.Zero)))
                {
                    CssClass = "bgRojo";
                    circleMarker.Color = circleMarker.FillColor = "#00ff00"; // Red = Max
                    circleMarker.Radius = 20;
                }
                else if (GasStationProducts.Any(x => x.Value <= (Products.FirstOrDefault(p => p.IdP == x.IdProducto)?.Avg ?? decimal.Zero)))
                {
                    CssClass = "bgAmarillo";
                    circleMarker.Color = circleMarker.FillColor = "#FFFF00"; // Yellow <= Avg
                    circleMarker.Radius = 10;
                }
                else
                {
                    CssClass = "bgNaranja";
                    circleMarker.Color = circleMarker.FillColor = "#FF8C00"; // Orange > Avg
                    circleMarker.Radius = 15;
                }

                MapModels.UILayers.Popup popup = new()
                {
                    Content = BuildPopupContent(GasStation, Products),
                };

                MapModels.UILayers.Tooltip tooltip = new()
                {
                    Content = $"<span class='{CssClass}'><b>{GasStation.RotuloTrimed}</b><span>",
                    Direction = MapModels.UILayers.Tooltip.Directions.Top,
                    Permanent = true,
                };

                await AddCircleMarker(circleMarker, popup, tooltip);
            }

            return null;

            static string BuildPopupContent(GasStationPrices.ViewModels.GasStationModel GasStation, IEnumerable<ProductLimits> Products)
            {
                System.Text.StringBuilder popupContent = new();
                _ = popupContent
                    .Append("<div class='container'>")
                    .Append($"<p>{GasStation.Localizacion}</p>");

                _ = popupContent
                    .Append("<div class='divTable'>")
                    .Append("<div class='divTableBody'>");

                for (int i = 0; i < GasStationPrices.Models.Minetur.ProductoPetrolifero.All.Count; i++)
                {
                    GasStationPrices.Models.Minetur.ProductoPetrolifero productoPetrolifero = GasStationPrices.Models.Minetur.ProductoPetrolifero.All[i];

                    decimal? GasVal = GasStation.GetProdById(productoPetrolifero.IdProducto);
                    if (!GasVal.HasValue)
                        continue;

                    string CssClass;
                    ProductLimits? productLimits = Products.FirstOrDefault(p => p.IdP == productoPetrolifero.IdProducto);
#pragma warning disable IDE0045 // Convert to conditional expression
                    if (GasVal.Value == (productLimits?.Min ?? decimal.Zero))
                        CssClass = "bgVerde";
                    else if (GasVal.Value == (productLimits?.Max ?? decimal.Zero))
                        CssClass = "bgRojo";
                    else if (GasVal.Value <= (productLimits?.Avg ?? decimal.Zero))
                        CssClass = "bgAmarillo";
                    else
                        CssClass = "bgNaranja";
#pragma warning restore IDE0045 // Convert to conditional expression

                    _ = popupContent
                        .Append($"<div class='divTableRow {CssClass}'>")
                        .Append($"<div class='divTableCell'>{productoPetrolifero.Nombre}</div>")
                        .Append($"<div class='divTableCell derecha'>{GasVal.Value.ToString("0.000 €")}</div>")
                        .Append("</div>"); // .divTableRow
                }

                _ = popupContent
                    .Append("</div>")   // .divTableBody
                    .Append("</div>")   // .divTable
                    .Append("</div>");  // .container

                return popupContent.ToString();
            }
        }
    }
}
internal record ProductLimits(GasStationPrices.Constants.ProductoPetroliferoId IdP, decimal? Min, decimal? Avg, decimal? Max);
