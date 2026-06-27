using LeafletForBlazor;
using Microsoft.AspNetCore.Components;

namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent
{
    // Samples obtained from https://github.com/ichim/LeafletForBlazor-NuGet/issues/75

    #region Initialization

    private readonly RealTimeMap.LoadParameters parameters = new()  //general map settings
    {
        basemap = new RealTimeMap.Basemap()
        {
            basemapLayers = [
                new RealTimeMap.BasemapLayer()
                {
                    url = "http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",
                    attribution = "©Open Street Map",
                    title = "Open Street Map",
                    detectRetina = true,
                },
            ]
        },
        location = new RealTimeMap.Location()
        {
            latitude = (double)Travel.Constants.Earth.Burgos.Lat,
            longitude = (double)Travel.Constants.Earth.Burgos.Lon,
        },
        zoomLevel = 14,
    };

    private readonly RealTimeMap.MapOptions options = new()
    {
        interactionOptions = new()
        {
            doubleClickZoom = true,
            dragging = true,
            shiftBoxZoom = false,
            trackResize = true,
        },
        keyboardNavigationOptions = new()
        {
            keyboard = false,
            keyboardPanDelta = 0
        },
    };

    #endregion

    private readonly string[] ColorsForRoutes = ["#007FFF", "#0074EA", "#0069D5", "#005EC0", "#0053AB", "#004896", "#003D81", "#00326C"];

    private RealTimeMap realTimeMap = default!;

    private readonly LeafletForBlazor.Components.StreamLegend.ContentStyle contentStyle = new()
    {
        contentPadding = new LeafletForBlazor.Components.StreamLegend.ContentPadding()
        {
            paddingLeft = 20,  //values ​​in pixels, default value is 10px
            paddingRight = 20, //values ​​in pixels, default value is 10px
            paddingTop = 15    //values ​​in pixels, default value is 10px
        },
        labelStyle = new LeafletForBlazor.Components.StreamLegend.LabelStyle()
        {
            fontColor = "#626262",
            fontFamily = "Verdana",
            fontSize = 14,
            fontWeight = "bold",
            fontStyle = "italic",
            paddingLeft = 20,
        },
    };

    // realTimeMap.Geometric.Points.Appearance(item => !(item.type == "red" || item.type == "green" || item.type == "blue")).pattern = new RealTimeMap.PointSymbol() { radius = 8, color = "#28ffff", opacity = 0.68, fillColor = "orange", weight = 2, fillOpacity = 0.68 };
    // realTimeMap.Geometric.Points.Appearance(item => item.type == "red").pattern = new RealTimeMap.PointSymbol() { radius = 8, color = "rgb(200,100,0)", opacity = 0.68, fillColor = "red", weight = 4, fillOpacity = 0.68 };
    // realTimeMap.Geometric.Points.Appearance(item => item.type == "green").pattern = new RealTimeMap.PointSymbol() { radius = 10, color = "white", opacity = 0.68, fillColor = "green", weight = 2, fillOpacity = 0.68 };
    // realTimeMap.Geometric.Points.Appearance(item => item.type == "blue").pattern = new RealTimeMap.PointSymbol() { radius = 12, color = "#28ffff", opacity = 0.68, fillColor = "blue", weight = 2, fillOpacity = 0.68 };

    [Inject] private GasStationPrices.Services.GasStationPricesService GasStationPricesService { get; set; } = default!;
    [Inject] private Travel.Services.Routing.RoutingService RoutingService { get; set; } = default!;

    /// <summary>
    /// Gets or sets the height of the <see cref="RealTimeMap" />.
    /// </summary>
    /// <remarks>
    /// Default value is <see langword="null" />.
    /// </remarks>
    [Parameter] public string? Height { get; set; } = "calc(100vh - 6rem)";

    /// <summary>
    /// Gets or sets the width of the <see cref="RealTimeMap" />.
    /// </summary>
    /// <remarks>
    /// Default value is <see langword="null" />.
    /// </remarks>
    [Parameter] public string? Width { get; set; } = "calc(100vw - 18rem)";

    /// <summary>
    /// Gets or sets the zoom level of the <see cref="RealTimeMap" />.
    /// </summary>
    /// <remarks>
    /// Default value is 14.
    /// </remarks>
    [Parameter] public int Zoom { get; set; } = 18;

    public static async Task OnAfterMapLoaded(RealTimeMap.MapEventArgs args)
    {
        if (args.sender != null)
            LoadGasStationAppearances(args.sender);
    }

    private static void OnClickMap(RealTimeMap.ClicksMapArgs args) { }

    private static void OnDoubleClickMap(RealTimeMap.ClicksMapArgs args) { }

    private void OnZoomLevelEndChange(RealTimeMap.MapZoomEventArgs args) => Zoom = args.zoomLevel;

    private async Task ClearMapAsync()
    {
        await realTimeMap.Geometric.DisplayPolylinesFromArray.deleteConnectors();
        await realTimeMap.Geometric.Points.delete();
    }

    /// <summary>
    /// This method can be used to perform actions after the map has loaded
    /// </summary>
    /// <param name="sender"></param>
    private static void LoadGasStationAppearances(RealTimeMap sender)
    {
        // "Cheap" : "Rest"
        sender.Geometric.Points.Appearance(item => item.type == "Cheap").pattern = new RealTimeMap.PointSymbol() { color = "white", fillColor = "green", fillOpacity = 0.68, opacity = 0.68, radius = 12, weight = 2 };
        sender.Geometric.Points.Appearance(item => item.type == "Other").pattern = new RealTimeMap.PointSymbol() { color = "blue", fillColor = "orange", fillOpacity = 0.68, opacity = 0.68, radius = 10, weight = 2 };

        //sender.Geometric.Points.clusteringAfterCollectionUpdate = true;
        //sender.Geometric.Points.clusteringConfiguration = new LeafletForBlazor.RealTime.points.ClusteringConfiguration()
        //{
        //    maxClusterRadius = 120,         // Maximum radius of a cluster when it is not zoomed in px
        //    showCoverageOnHover = true,     // Show coverage on hover
        //    spiderfyOnMaxZoom = true,       // Spiderfy markers when zoomed in
        //    zoomToBoundsOnClick = false,    // Zoom to bounds when clicking on a cluster
        //};
    }

    public async Task<string?> LoadRoutesAndGasStationsAsync(GasStationPrices.ViewModels.TravelQueryModel model, CancellationToken cancellationToken)
    {
        await ClearMapAsync();

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

        Travel.Models.Bounds ourBounds = ComputeBoundsFromRoutes(res, cancellationToken);

        await LoadGasStationsIntoMapAsync(model, ourBounds, cancellationToken);

        return null;

        async Task LoadRoutesDataIntoMapAsync(
            IReadOnlyList<(string NombreRuta
            , double[,] Coordenadas)> res
            , CancellationToken cancellationToken)
        {
            for (int i = 0; i < res.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                (string? NombreRuta, double[,]? Coordenadas) = res[i];
                await realTimeMap.Geometric.DisplayPolylinesFromArray.addConnector(
                    arrayPolyline: Coordenadas
                    , symbol: new RealTimeMap.PolylineSymbol() { color = ColorsForRoutes[i], /*smoothFactor = 1.0,*/ /*opacity = 1.0,*/ weight = 5, }
                    , start: 1);
            }
        }

        Travel.Models.Bounds ComputeBoundsFromRoutes(
            IReadOnlyList<(string NombreRuta
            , double[,] Coordenadas)> res
            , CancellationToken cancellationToken)
        {
            // Take inverse limits, so any obtained point will be used
            RealTimeMap.Bounds routeBounds = new()
            {
                northEast = new RealTimeMap.Location() { latitude = (double)Travel.Models.Bounds.Limits.South, longitude = (double)Travel.Models.Bounds.Limits.West, }, // South West limits
                southWest = new RealTimeMap.Location() { latitude = (double)Travel.Models.Bounds.Limits.North, longitude = (double)Travel.Models.Bounds.Limits.East, },   // North East limits
            };

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
                            if (v > routeBounds.northEast.latitude)
                                routeBounds.northEast.latitude = v;
                            if (v < routeBounds.southWest.latitude)
                                routeBounds.southWest.latitude = v;
                        }
                        else // (j == 1)
                        {
                            // longitude
                            if (v > routeBounds.northEast.longitude)
                                routeBounds.northEast.longitude = v;
                            if (v < routeBounds.southWest.longitude)
                                routeBounds.southWest.longitude = v;
                        }
                    }
                }
            }

            realTimeMap.View.setBounds = routeBounds;

            Travel.Models.Bounds boundsForGasStations = new(
                NorthEast: new Travel.Models.Location((decimal)routeBounds.northEast.latitude, (decimal)routeBounds.northEast.longitude),
                SouthWest: new Travel.Models.Location((decimal)routeBounds.southWest.latitude, (decimal)routeBounds.southWest.longitude));

            return boundsForGasStations;
        }

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
                let v = gasStations.Select(x => x.GetProdById(p.IdProducto)).Where(x => x.HasValue)
                select new
                {
                    IdP = p.IdProducto,
                    Min = v.Min(),
                    Avg = v.Average(),
                };

            IReadOnlyList<string> plantillaPrecios = [..
                from a in GasStationPrices.Models.Minetur.ProductoPetrolifero.All
                where model.PetroleumProductsSelectedIds.Contains(a.IdProducto)
                select "<li><b>" + a.Abreviatura.ToUpperInvariant() + ": </b> " + "${value." + a.Abreviatura.ToLowerInvariant() + "} €" + "</li>"
            ];

            realTimeMap.Geometric.Points.Appearance(p => (new[] { "Cheap", "Other" }).Contains(p.type)).pattern = new RealTimeMap.PointTooltip()
            {
                content = "<h2>${value.rotulo}</h2><h4>${value.localizacion}<h4><ul>" + string.Concat(plantillaPrecios) + "</ul>",
                offset = [0, -50], // default new double[2]
                opacity = 1.0,     // default 0.9
                permanent = false, // default true
            };

            List<RealTimeMap.StreamPoint> gasStationPoints = [..
                from g in gasStations
                let any = g.AllProducts().Any(x => x.Value <= Products.First(p => p.IdP == x.IdProducto).Avg)
                let pt = any ? "Cheap" : "Other"
                select new RealTimeMap.StreamPoint()
                {
                    guid = Guid.NewGuid(),
                    latitude = (double)g.Lat,
                    longitude = (double)g.Lon,
                    //timestamp =,
                    type = pt,
                    value = g,
                }];

            await realTimeMap.Geometric.Points.upload(gasStationPoints);
        }
    }
}
