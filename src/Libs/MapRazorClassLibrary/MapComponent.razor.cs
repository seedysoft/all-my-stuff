using LeafletForBlazor;
using Microsoft.AspNetCore.Components;

namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent
{
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

    private static void LoadGasStationAppearances(RealTimeMap sender)
    {
        //sender.Geometric.Points.Appearance(item => item.type == "gas-station").pattern = new RealTimeMap.PointIcon()
        //{
        //    iconUrl = "https://www.google.com/s2/favicons?domain=$repsol&sz=16",
        //};
    }

    private static void OnClickMap(RealTimeMap.ClicksMapArgs args) { }

    private static void OnDoubleClickMap(RealTimeMap.ClicksMapArgs args) { }

    public async Task ClearMapAsync()
    {
        await realTimeMap.Geometric.DisplayPolylinesFromArray.deleteConnectors();
        await realTimeMap.Geometric.Points.delete();
    }

    public async Task<string?> LoadRoutesAndGasStationsAsync(GasStationPrices.ViewModels.TravelQueryModel model, CancellationToken cancellationToken)
    {
        await ClearMapAsync();

        IList<(string NombreRuta, double[,] Coordenadas)> res;

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

        await LoadRoutesDataIntoMapAsync(res);

        Travel.Models.Bounds ourBounds = ComputeBoundsFromRoutes(res);

        await LoadGasStationsIntoMapAsync(model, ourBounds, cancellationToken);

        return null;

        // Samples obtained from https://github.com/ichim/LeafletForBlazor-NuGet/issues/75

        async Task LoadRoutesDataIntoMapAsync(
            IList<(string NombreRuta,
            double[,] Coordenadas)> res)
        {
            for (int i = 0; i < res.Count; i++)
            {
                (string? NombreRuta, double[,]? Coordenadas) = res[i];

                realTimeMap.Geometric.Points.Appearance(item => item.type != "gas-station").pattern = new RealTimeMap.PointSymbol()
                {
                    radius = 6, // default 4
                    fillColor = ColorsForRoutes[i],
                    //color = ColorsForRoutes[i], // default null
                    //fillOpacity = 1.0, // default 1.0
                    opacity = 1.0, // default 0.0
                    weight = 1, // default 0
                };

                await realTimeMap.Geometric.DisplayPolylinesFromArray.addConnector(arrayPolyline: Coordenadas, start: 1);
            }
        }

        Travel.Models.Bounds ComputeBoundsFromRoutes(
            IList<(string NombreRuta,
            double[,] Coordenadas)> res)
        {
            RealTimeMap.Bounds routeBounds = new()
            {
                northEast = new RealTimeMap.Location() { latitude = -90.0, longitude = -180.0, }, // South West limits
                southWest = new RealTimeMap.Location() { latitude = 90.0, longitude = 180.0, },   // North East limits
            };

            foreach ((string NombreRuta, double[,] Coordenadas) in res)
            {
                for (int i = 0; i < Coordenadas.GetLength(0); i++)
                {
                    for (int j = 0; j < Coordenadas.GetLength(1); j++)
                    {
                        double v = (double)Coordenadas[i, j];

                        if (j == 0)
                        {
                            // latitude
                            if (v > routeBounds.northEast.latitude)
                                routeBounds.northEast.latitude = v;
                            if (v < routeBounds.southWest.latitude)
                                routeBounds.southWest.latitude = v;
                        }
                        else
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

            Travel.Models.Bounds boundsForGasStations = new()
            {
                NorthEast = new Travel.Models.Location(lat: (decimal)routeBounds.northEast.latitude, lon: (decimal)routeBounds.northEast.longitude),
                SouthWest = new Travel.Models.Location(lat: (decimal)routeBounds.southWest.latitude, lon: (decimal)routeBounds.southWest.longitude),
            };

            return boundsForGasStations;
        }

        async Task LoadGasStationsIntoMapAsync(
            GasStationPrices.ViewModels.TravelQueryModel model,
            Travel.Models.Bounds bounds,
            CancellationToken cancellationToken)
        {
            var gasStationPoints = (await GasStationPricesService.GetNearGasStationsAsync(bounds, model.MaxDistanceInKm, cancellationToken))
                .Select(x => new RealTimeMap.StreamPoint()
                {
                    guid = Guid.NewGuid(),
                    latitude = (double)x.Lat,
                    longitude = (double)x.Lon,
                    //timestamp =,
                    type = "gas-station",
                    value = x,
                })
                .ToList();

            IEnumerable<string> plantillaPrecios =
                from a in GasStationPrices.Models.Minetur.ProductoPetrolifero.All
                where model.PetroleumProductsSelectedIds.Contains(a.IdProducto)
                select "<li><b>" + a.Abreviatura.ToUpperInvariant() + ": </b> " + "${value." + a.Abreviatura.ToLowerInvariant() + "} €" + "</li>";

            realTimeMap.Geometric.Points.Appearance(item => item.type == "gas-station").pattern = new RealTimeMap.PointTooltip()
            {
                content = "<h3>${value.rotulo}</h3><h5>${value.localizacion}<h5><ul>" + string.Concat(plantillaPrecios) + "</ul>",
                permanent = false, // default true
                offset = [0, -50], // default new double[2]
                opacity = 1.0, // default 0.9
            };

            await realTimeMap.Geometric.Points.upload(gasStationPoints);
        }
    }
}
