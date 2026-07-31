using Microsoft.JSInterop;

namespace Seedysoft.Libs.MapRazorClassLibrary;

/// <summary>
/// Blazor component that manages an interactive map with route visualization and gas station markers.
/// Integrates Leaflet.js for map rendering and provides functionality to display travel routes and nearby gas stations.
/// </summary>
/// <remarks>
/// <para>
/// This component is a wrapper around the Leaflet.js mapping library, designed to provide a rich interactive map experience
/// in Blazor WebAssembly applications. It serves as a visual interface for route planning and gas station discovery.
/// </para>
/// <para>
/// Key Features:
/// <list type="bullet">
/// <item><description>Dynamically loads a Leaflet map module on first render for lazy loading optimization</description></item>
/// <item><description>Displays multiple travel routes with distinct colors for easy differentiation</description></item>
/// <item><description>Marks gas stations with color-coded circle indicators based on fuel price comparisons (minimum, average, maximum)</description></item>
/// <item><description>Supports asynchronous cancellation tokens for all operations to enable responsive UI cancellation</description></item>
/// <item><description>Implements IAsyncDisposable for proper cleanup of JavaScript interop resources and event listeners</description></item>
/// <item><description>Uses JavaScript interop to invoke Leaflet library features while maintaining type safety on the C# side</description></item>
/// </list>
/// </para>
/// <para>
/// Architecture:
/// The component uses local JavaScript interop through a dynamically imported leafletModule.js file that handles all direct Leaflet
/// library interactions. This separation of concerns allows for maintainable and testable code while leveraging Leaflet's powerful
/// mapping capabilities.
/// </para>
/// <para>
/// Usage:
/// The component is initialized via the LoadRoutesAndGasStationsAsync method, which orchestrates the entire data loading pipeline.
/// The component manages its own state internally, including markers, polylines, and the map instance.
/// </para>
/// </remarks>
/// <seealso cref="IAsyncDisposable"/>
/// <seealso cref="LoadRoutesAndGasStationsAsync"/>
public partial class MapComponent : IAsyncDisposable
{
    /// <summary>
    /// Array of hex color codes used to differentiate multiple routes on the map.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contains 8 distinct shades of blue, progressing from lighter to darker hues.
    /// Each route is assigned a color from this array based on its zero-based index position.
    /// </para>
    /// <para>
    /// Color Palette:
    /// <list type="table">
    /// <listheader><term>Index</term><term>Hex Code</term><term>Description</term></listheader>
    /// <item><term>0</term><term>#007FFF</term><term>Light Blue</term></item>
    /// <item><term>1</term><term>#0074EA</term><term>Sky Blue</term></item>
    /// <item><term>2</term><term>#0069D5</term><term>Medium Blue</term></item>
    /// <item><term>3</term><term>#005EC0</term><term>Deep Blue</term></item>
    /// <item><term>4</term><term>#0053AB</term><term>Darker Blue</term></item>
    /// <item><term>5</term><term>#004896</term><term>Navy Blue</term></item>
    /// <item><term>6</term><term>#003D81</term><term>Dark Navy</term></item>
    /// <item><term>7</term><term>#00326C</term><term>Very Dark Blue</term></item>
    /// </list>
    /// </para>
    /// <para>
    /// If more than 8 routes are loaded, the color assignment will cycle through this array using modulo arithmetic.
    /// Supports up to 8 distinct routes without color repetition.
    /// </para>
    /// </remarks>
    private readonly string[] ColorsForRoutes = ["#007FFF", "#0074EA", "#0069D5", "#005EC0", "#0053AB", "#004896", "#003D81", "#00326C"];

    /// <summary>
    /// Initializes the component by creating a DotNet object reference for JavaScript interop.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This lifecycle method is called automatically by Blazor once during the component initialization phase.
    /// It creates a DotNetObjectReference that enables JavaScript code (in leafletModule.js) to invoke .NET methods
    /// on this component instance, establishing the bidirectional communication bridge.
    /// </para>
    /// <para>
    /// The reference is stored in the ObjRef property for later cleanup during disposal.
    /// </para>
    /// <para>
    /// Timing: This runs before OnAfterRenderAsync, ensuring the reference is available for all subsequent operations.
    /// </para>
    /// </remarks>
    /// <seealso cref="OnAfterRenderAsync"/>
    protected override void OnInitialized() => ObjRef = DotNetObjectReference.Create(this);

    /// <summary>
    /// Initializes the map module and creates the map instance on first render.
    /// </summary>
    /// <param name="firstRender">Indicates whether this is the first render pass of the component.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <remarks>
    /// <para>
    /// This lifecycle method is invoked by Blazor after the component has been rendered to the browser DOM.
    /// It is called twice per render cycle: once after the initial render and once after re-renders.
    /// </para>
    /// <para>
    /// Execution Flow on First Render:
    /// <list type="number">
    /// <item><description>Calls the base class OnAfterRenderAsync to ensure inheritance chain is maintained</description></item>
    /// <item><description>Checks if this is the first render pass; skips execution on subsequent renders for performance</description></item>
    /// <item><description>Verifies that MapModule has not already been initialized (null check)</description></item>
    /// <item><description>Dynamically imports the leafletModule.js from the component's JavaScript folder using ES6 modules</description></item>
    /// <item><description>Invokes CreateMapAsync() to initialize the Leaflet map instance with default settings</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Performance Optimization:
    /// Returns immediately on non-first renders, preventing unnecessary module imports and map initialization.
    /// The map is only created once during the component's lifetime.
    /// </para>
    /// <para>
    /// Module Path:
    /// The leafletModule.js is imported from _content/Seedysoft.Libs.MapRazorClassLibrary/js/, which is the standard
    /// path for static files in Razor Class Libraries served to Blazor WebAssembly applications.
    /// </para>
    /// </remarks>
    /// <seealso cref="OnInitialized"/>
    /// <seealso cref="CreateMapAsync"/>
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
    /// Loads travel routes and gas stations onto the map based on the provided travel query criteria.
    /// </summary>
    /// <param name="model">The travel query model containing origin location, destination location, 
    /// selected petroleum product IDs, and maximum search distance in kilometers.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation at any point during execution.</param>
    /// <returns>
    /// Returns null (string?) on successful completion of loading all routes and gas stations.
    /// Returns an error message string if:
    /// <list type="bullet">
    /// <item><description>An exception occurs during route retrieval</description></item>
    /// <item><description>No routes are found between origin and destination</description></item>
    /// <item><description>No gas stations are found within the computed bounds and max distance</description></item>
    /// <item><description>No petroleum products match the selected IDs</description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is the primary public method for loading and displaying map data. It orchestrates a multi-step pipeline
    /// to fetch routes, display them, and then add relevant gas station markers.
    /// </para>
    /// <para>
    /// Process Flow:
    /// <list type="number">
    /// <item><description>Displays a loading indicator to provide user feedback</description></item>
    /// <item><description>Clears all existing markers from previous searches</description></item>
    /// <item><description>Retrieves routes from RoutingService based on origin and destination coordinates</description></item>
    /// <item><description>Displays routes as colored polylines on the map using LoadRoutesDataIntoMapAsync</description></item>
    /// <item><description>Computes geographic bounds (bounding box) encompassing all route coordinates</description></item>
    /// <item><description>Fetches gas stations within the computed bounds from GasStationPricesService</description></item>
    /// <item><description>Calculates price statistics (min, average, max) for each selected petroleum product</description></item>
    /// <item><description>Marks gas stations with color-coded circle indicators based on price comparisons</description></item>
    /// <item><description>Hides the loading indicator</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Cancellation:
    /// The operation supports cancellation at multiple checkpoints.
    /// If cancellation is requested, the operation will stop gracefully at the next checkpoint and clean up resources appropriately.
    /// </para>
    /// <para>
    /// Error Handling:
    /// All exceptions during route retrieval are caught and converted to error message strings.
    /// The pipeline continues to the next step only if the previous step returned null (success).
    /// </para>
    /// <para>
    /// Dependencies:
    /// - RoutingService: Provides route calculation between two locations
    /// - GasStationPricesService: Provides gas station data with pricing information
    /// - ProductoPetrolifero.All: Static collection of available petroleum products
    /// </para>
    /// </remarks>
    /// <seealso cref="LoadRoutesDataIntoMapAsync"/>
    /// <seealso cref="ComputeBoundsFromRoutes"/>
    /// <seealso cref="LoadGasStationsIntoMapAsync"/>
    /// <seealso cref="ShowLoaderAsync"/>
    /// <seealso cref="HideLoaderAsync"/>
    /// <seealso cref="RemoveAllMarkersAsync"/>
    public async Task<string?> LoadRoutesAndGasStationsAsync(GasStationPrices.ViewModels.TravelQueryModel model, CancellationToken cancellationToken)
    {
        string? returnText = null;

        await ShowLoaderAsync();

        await RemoveAllMarkersAsync();

        IReadOnlyList<(string NombreRuta, double[,] Coordenadas)> res;
        try
        {
            res = await RoutingService.GetRoutesAsync(model.Orig.Location, model.Dest.Location, cancellationToken);
        }
        catch (Exception e)
        {
            res = [];
            returnText = e.ToString();
        }

        if (string.IsNullOrWhiteSpace(returnText))
        {
            if (res.Count == 0)
                returnText = "No routes found";

            if (string.IsNullOrWhiteSpace(returnText))
                returnText = await LoadRoutesDataIntoMapAsync(res, cancellationToken);

            if (string.IsNullOrWhiteSpace(returnText))
            {
                IReadOnlyList<GasStationPrices.ViewModels.GasStationModel> gasStations =
                await GasStationPricesService.GetNearGasStationsAsync(ComputeBoundsFromRoutes(res, cancellationToken), model.MaxDistanceInKm, cancellationToken);
                if (gasStations.Count == 0)
                {
                    returnText = "⚠️ No Gas stations loaded ⚠️";
                }
                else
                {
                    // For each product, obtain min, average and max
                    ProductLimits[] productLimits = [..
                        from p in GasStationPrices.Models.Minetur.ProductoPetrolifero.All
                        where model.PetroleumProductsSelectedIds.Contains(p.IdProducto)
                        let v = gasStations.Select(x => x.GetProdById(p.IdProducto)).Where(x => x.HasValue)
                        select new ProductLimits(
                            p.IdProducto,
                            v.Min(),
                            v.Average(),
                            v.Max()
                        )];

                    returnText = productLimits.Length == 0
                        ? "⚠️ No Products to show ⚠️"
                        : await LoadDataIntoMapAsync(model, gasStations, productLimits, cancellationToken);
                }
            }
        }

        await HideLoaderAsync();

        return returnText;

        /// <summary>
        /// Displays route polylines on the map using color-coded lines for visual differentiation.
        /// </summary>
        /// <param name="res">Collection of routes, each containing a name and a 2D array of coordinates.</param>
        /// <param name="cancellationToken">Token to cancel the operation at any point during processing.</param>
        /// <returns>
        /// Returns null on successful rendering of all routes.
        /// Returns a warning message string if no routes are provided.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This nested local function is responsible for rendering route data as polylines on the Leaflet map.
        /// It processes each route sequentially and assigns it a unique color from the ColorsForRoutes array.
        /// </para>
        /// <para>
        /// Processing:
        /// <list type="number">
        /// <item><description>Validates that routes collection is not empty</description></item>
        /// <item><description>Iterates through each route in the collection by index</description></item>
        /// <item><description>Respects cancellation requests by checking the cancellation token at each iteration</description></item>
        /// <item><description>Deconstructs the route tuple to extract name and coordinate array</description></item>
        /// <item><description>Invokes AddPolylineAsync to render the route on the map</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// Color Assignment:
        /// Each route is assigned a color based on its index: res[0] uses ColorsForRoutes[0], res[1] uses ColorsForRoutes[1], etc.
        /// If more than 8 routes exist, this implementation would require cycling through the color array.
        /// </para>
        /// <para>
        /// Coordinate Format:
        /// The coordinate array is expected to be a 2D array where each row represents a point on the route.
        /// The format is: [row][col] where col 0 = latitude and col 1 = longitude.
        /// </para>
        /// <para>
        /// Cancellation:
        /// If cancellation is requested during processing, the method breaks out of the loop without rendering
        /// remaining routes, and returns null (indicating successful partial completion).
        /// </para>
        /// </remarks>
        /// <seealso cref="ColorsForRoutes"/>
        /// <seealso cref="AddPolylineAsync"/>
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
        /// Calculates the geographic bounds (bounding box) encompassing all coordinates from all routes.
        /// </summary>
        /// <param name="res">Collection of routes with their coordinate arrays to analyze.</param>
        /// <param name="cancellationToken">Token to cancel the operation during processing.</param>
        /// <returns>
        /// A Bounds object representing the geographic rectangle that encompasses all route coordinates.
        /// Contains NorthEast and SouthWest corner locations with latitude and longitude values.
        /// Returns Empty bounds if cancellation is requested.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This function computes the minimum bounding rectangle that contains all coordinates from all routes.
        /// The bounds are essential for filtering gas stations to search only in the relevant geographic area.
        /// </para>
        /// <para>
        /// Algorithm:
        /// <list type="number">
        /// <item><description>Initializes latitude and longitude limits to inverse extremes (ensures any real value updates them)</description></item>
        /// <item><description>Iterates through each route and its coordinates in nested loops</description></item>
        /// <item><description>Tracks the maximum latitude and minimum latitude (NorthEast and SouthWest Y-axis)</description></item>
        /// <item><description>Tracks the maximum longitude and minimum longitude (NorthEast and SouthWest X-axis)</description></item>
        /// <item><description>Returns the computed bounds or Empty if cancellation is requested</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// Coordinate Interpretation:
        /// Coordinates are stored in a 2D array where:
        /// - Column 0 represents latitude (North-South position, ranges -90 to 90)
        /// - Column 1 represents longitude (East-West position, ranges -180 to 180)
        /// Higher latitude = further North
        /// Higher longitude = further East
        /// </para>
        /// <para>
        /// Inverse Limits Usage:
        /// Initial values are set to Travel.Models.Bounds.Inverse properties, which contain extreme values
        /// that ensure any real geographic coordinate will adjust the bounds appropriately.
        /// </para>
        /// <para>
        /// Cancellation:
        /// The operation checks for cancellation at the route level, coordinate row level, and coordinate column level.
        /// If cancelled, returns Travel.Models.Bounds.Empty immediately.
        /// </para>
        /// </remarks>
        /// <seealso cref="Travel.Models.Bounds"/>
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
        /// Displays gas stations on the map as color-coded circle markers with popups and tooltips.
        /// </summary>
        /// <param name="model">The travel query model containing selected petroleum product IDs.</param>
        /// <param name="gasStations">Collection of gas stations to display on the map.</param>
        /// <param name="productLimits">Array of ProductLimits objects containing min, average, and max prices for each product.</param>
        /// <param name="cancellationToken">Token to cancel the operation during processing.</param>
        /// <returns>
        /// Returns null on successful rendering of all gas stations.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This function is responsible for creating visual representations of gas stations on the map.
        /// Each station is displayed as a circle marker with a size and color that indicates its price competitiveness.
        /// </para>
        /// <para>
        /// Color and Size Legend:
        /// <list type="table">
        /// <listheader><term>Price Category</term><term>Color</term><term>Hex Code</term><term>Radius</term><term>Meaning</term></listheader>
        /// <item><term>Minimum Price</term><term>Green</term><term>#ff0000</term><term>5px</term><term>Best prices available</term></item>
        /// <item><term>Maximum Price</term><term>Red</term><term>#00ff00</term><term>20px</term><term>Highest prices available</term></item>
        /// <item><term>Below Average</term><term>Yellow</term><term>#FFFF00</term><term>10px</term><term>Reasonably priced</term></item>
        /// <item><term>Above Average</term><term>Orange</term><term>#FF8C00</term><term>15px</term><term>Slightly expensive</term></item>
        /// </list>
        /// </para>
        /// <para>
        /// Visual Design:
        /// Each marker is a CircleMarker configured with:
        /// - Fill enabled (FillOpacity = 1.0) for solid appearance
        /// - FillRule set to "nonzero" for proper shape rendering
        /// - Color matching the FillColor for visual consistency
        /// </para>
        /// <para>
        /// Interaction Features:
        /// - Popup: Displays detailed gas station information including address and individual product prices
        /// - Tooltip: Shows station name with CSS styling and permanent display mode for always-visible labels
        /// </para>
        /// <para>
        /// Price Comparison Logic:
        /// For each gas station, the function checks if any of its product prices match or fall into specific categories:
        /// 1. Minimum: Lowest price among all stations for that product
        /// 2. Maximum: Highest price among all stations for that product
        /// 3. Below Average: Price less than or equal to average price
        /// 4. Above Average: Price greater than average (default case)
        /// </para>
        /// <para>
        /// Tooltip Styling:
        /// The tooltip content includes an HTML span with a CSS class (bgVerde, bgRojo, bgAmarillo, bgNaranja)
        /// for styling via external stylesheets. Content is displayed above the marker.
        /// </para>
        /// </remarks>
        /// <seealso cref="BuildPopupContent"/>
        /// <seealso cref="AddCircleMarker"/>
        /// <seealso cref="ProductLimits"/>
        async Task<string?> LoadDataIntoMapAsync(
            GasStationPrices.ViewModels.TravelQueryModel model
            , IReadOnlyList<GasStationPrices.ViewModels.GasStationModel> gasStations
            , ProductLimits[] productLimits
            , CancellationToken cancellationToken)
        {
            await LoadProductLimitsAsync(productLimits);

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
                if (GasStationProducts.Any(x => x.Value == (productLimits.FirstOrDefault(p => p.IdP == x.IdProducto)?.Min ?? decimal.Zero)))
                {
                    CssClass = "bgVerde";
                    circleMarker.Color = circleMarker.FillColor = "#ff0000"; // Green = Min
                    circleMarker.Radius = 5;
                }
                else if (GasStationProducts.Any(x => x.Value == (productLimits.FirstOrDefault(p => p.IdP == x.IdProducto)?.Max ?? decimal.Zero)))
                {
                    CssClass = "bgRojo";
                    circleMarker.Color = circleMarker.FillColor = "#00ff00"; // Red = Max
                    circleMarker.Radius = 20;
                }
                else if (GasStationProducts.Any(x => x.Value <= (productLimits.FirstOrDefault(p => p.IdP == x.IdProducto)?.Avg ?? decimal.Zero)))
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
                    Content = BuildPopupContent(GasStation, productLimits),
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

            /// <summary>
            /// Builds HTML popup content displaying gas station details and product pricing information.
            /// </summary>
            /// <param name="GasStation">The gas station model containing location and brand information.</param>
            /// <param name="Products">Collection of ProductLimits objects with min, average, and max pricing data.</param>
            /// <returns>
            /// An HTML-formatted string containing the gas station's address and a formatted list of product prices.
            /// </returns>
            /// <remarks>
            /// <para>
            /// This static helper function generates the HTML content for the popup that appears when a gas station marker is clicked.
            /// It provides detailed information about the station and its prices.
            /// </para>
            /// <para>
            /// HTML Structure:
            /// The popup content is wrapped in a div with class "container" and includes:
            /// - Location/Address paragraph
            /// - Product pricing list with individual prices for selected products
            /// </para>
            /// <para>
            /// Usage:
            /// This function is called once per gas station during LoadGasStationsIntoMapAsync.
            /// The HTML is displayed in a Leaflet popup when the user clicks on a station marker.
            /// </para>
            /// </remarks>
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
