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
                //new RealTimeMap.BasemapLayer()
                //{
                //    url = "http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",
                //    attribution = "©Open Street Map",
                //    title = "Open Street Map",
                //    detectRetina = true,
                //},
                //new RealTimeMap.BasemapLayer()
                //{
                //    url = "https://tile.opentopomap.org/{z}/{x}/{y}.png",
                //    attribution = "Open Topo",
                //    title = "Open Topo",
                //    detectRetina = true,
                //},
                // new RealTimeMap.BasemapLayer()
                //{
                //    url = "https://tile.thunderforest.com/cycle/{z}/{x}/{y}.png?apikey=" + openCycleMapAPIKey,
                //    attribution = "©Open Cycle Map",
                //    title = "Open Cycle Map"
                //},

                //new RealTimeMap.BasemapLayer(){
                //    url="https://tms-ign-base.idee.es/1.0.0/IGNBaseGris/{z}/{x}/{-y}.jpeg",
                //    attribution="Mapa Base de España – IGNBaseGris",
                //    title="Mapa Base de España – IGNBaseGris",
                //    detectRetina=true,
                //},
                //new RealTimeMap.BasemapLayer(){
                //    url="https://tms-ign-base.idee.es/1.0.0/IGNBaseOrto/{z}/{x}/{-y}.png",
                //    attribution="Mapa Base de España - IGNBaseOrto",
                //    title="Mapa Base de España - IGNBaseOrto",
                //    detectRetina=true,
                //},
                //new RealTimeMap.BasemapLayer(){
                //    url="https://tms-ign-base.idee.es/1.0.0/IGNBaseSimplificado/{z}/{x}/{-y}.png",
                //    attribution="Mapa Base de España - IGNBaseSimplificado",
                //    title="Mapa Base de España - IGNBaseSimplificado",
                //    detectRetina=true,
                //},
                new RealTimeMap.BasemapLayer(){
                    url="https://tms-ign-base.idee.es/1.0.0/IGNBaseTodo/{z}/{x}/{-y}.jpeg",
                    attribution="Mapa Base de España - IGNBaseTodo",
                    title="Mapa Base de España - IGNBaseTodo",
                    detectRetina=true,
                },
                //new RealTimeMap.BasemapLayer(){
                //    url="https://xyz-mdt.idee.es/1.0.0/raster-dem/{z}/{x}/{y}.png",
                //    attribution="Modelo Digital de Elevaciones de España",
                //    title="Modelo Digital de Elevaciones de España",
                //    detectRetina=true,
                //},
                //new RealTimeMap.BasemapLayer(){
                //    url="https://tms-pnoa-ma.idee.es/1.0.0/pnoa-ma/{z}/{x}/{-y}.jpeg",
                //    attribution="Ortoimágenes MA de España (Sentinel2 y PNOA MA)",
                //    title="Ortoimágenes MA de España (Sentinel2 y PNOA MA)",
                //    detectRetina=true,
                //},
                //new RealTimeMap.BasemapLayer(){
                //    url="https://tms-relieve.idee.es/1.0.0/relieve/{z}/{x}/{-y}.jpeg",
                //    attribution="Relieve del terreno de España (procedente del MDT con paso de malla 25 m)",
                //    title="Relieve del terreno de España (procedente del MDT con paso de malla 25 m)",
                //    detectRetina=true,
                //},
                //new RealTimeMap.BasemapLayer(){
                //    url="https://tms-mapa-raster.ign.es/1.0.0/mapa-raster/{z}/{x}/{-y}.jpeg",
                //    attribution="Cartografía Ráster de España del IGN",
                //    title="Cartografía Ráster de España del IGN",
                //    detectRetina=true,
                //},
            ]
        },
        location = new RealTimeMap.Location()
        {
            latitude = GasStationPrices.Constants.Earth.Home.Center.Lat,
            longitude = GasStationPrices.Constants.Earth.Home.Center.Lng,
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

    private RealTimeMap realTimeMap = new();

    [Inject] private GasStationPrices.Services.GasStationPricesService GasStationPricesService { get; set; } = default!;
    [Inject] private GasStationPrices.Services.RoutesService RoutesService { get; set; } = default!;

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

        //IList<(string NombreRuta, double[][] Coordenadas)> res = await RoutesService.GetRoutesAsync(model, cancellationToken);
        IList<(string NombreRuta, double[,] Coordenadas)> res = await RoutesService.GetRoutesAsync(model, cancellationToken);

        if (res.Count == 0)
            return "No routes found";

        await LoadRouteDataIntoMapAsync(res);

        GasStationPrices.Models.Bounds ourBounds = ComputeBoundsFromRoutes(res);

        await LoadGasStationsIntoMapAsync(model, ourBounds, cancellationToken);

        return null;

        // Samples obtained from https://github.com/ichim/LeafletForBlazor-NuGet/issues/75

        //async Task LoadRouteDataIntoMapAsync(IList<(string NombreRuta, double[][] Coordenadas)> res)
        async Task LoadRouteDataIntoMapAsync(IList<(string NombreRuta, double[,] Coordenadas)> res)
        {
            for (int i = 0; i < res.Count; i++)
            {
                //(string? NombreRuta, double[][]? Coordenadas) = res[i];
                (string? NombreRuta, double[,]? Coordenadas) = res[i];

                await realTimeMap.Geometric.DisplayPolylinesFromArray.addConnector(arrayPolyline: Coordenadas, start: 1);

                //GasStationPrices.Models.GeoJSONLineStringClass inputPolygon = new()
                //{
                //    Geometry = new GasStationPrices.Models.LineStringGeometry() { Coordinates = Coordenadas, },
                //    Properties = new GasStationPrices.Models.Properties(NombreRuta),
                //};

                //GasStationPrices.Models.GeoJSONPolygonAppearanceClass polygonAppearance = new()
                //{
                //    Data = [inputPolygon],
                //    Name = NombreRuta,
                //    Symbology = new GasStationPrices.Models.PolygonSymbol()
                //    {
                //        Color = ColorsForRoutes[i],
                //        Opacity = 0.6,
                //        Weight = 8,
                //    },
                //    Tooltip = new GasStationPrices.Models.Tooltip()
                //    {
                //        Content = NombreRuta,
                //        CoordinateInversion = false,
                //        Offset = [0, -10],
                //        Opacity = 0.9f,
                //        Permanent = false,
                //        VisibilityZoomLevels = new GasStationPrices.Models.VisibilityZoomLevel()
                //        {
                //            MaxZoomLevel = 18,
                //            MinZoomLevel = 0,
                //        },
                //    }
                //};

                //await realTimeMap.Geometric.DataFromGeoJSON.addObject(polygonAppearance);
            }
        }

        //GasStationPrices.Models.Bounds ComputeBoundsFromRoutes(IList<(string NombreRuta, double[][] Coordenadas)> res)
        GasStationPrices.Models.Bounds ComputeBoundsFromRoutes(IList<(string NombreRuta, double[,] Coordenadas)> res)
        {
            RealTimeMap.Bounds routeBounds = new()
            {
                northEast = new RealTimeMap.Location()
                {
                    latitude = -90.0,
                    longitude = -60.0,
                },
                southWest = new RealTimeMap.Location()
                {
                    latitude = 90.0,
                    longitude = 60.0,
                },
            };
            foreach ((string NombreRuta, double[,] Coordenadas) in res)
            {
                for (int i = 0; i < Coordenadas.GetLength(0); i++)
                {
                    for (int j = 0; j < Coordenadas.GetLength(1); j++)
                    {
                        double v = Coordenadas[i, j];

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

            GasStationPrices.Models.Bounds boundsForGasStations = new()
            {
                NorthEast = new GasStationPrices.Models.Location()
                {
                    Latitude = routeBounds.northEast.latitude,
                    Longitude = routeBounds.northEast.longitude,
                },
                SouthWest = new GasStationPrices.Models.Location()
                {
                    Latitude = routeBounds.southWest.latitude,
                    Longitude = routeBounds.southWest.longitude,
                },
            };

            return boundsForGasStations;
        }

        async Task LoadGasStationsIntoMapAsync(GasStationPrices.ViewModels.TravelQueryModel model, GasStationPrices.Models.Bounds bounds, CancellationToken cancellationToken)
        {
            var gasStationPoints = (await GasStationPricesService.GetNearGasStationsAsync(bounds, model.MaxDistanceInKm, cancellationToken))
                .Select(x => new RealTimeMap.StreamPoint()
                {
                    guid = Guid.NewGuid(),
                    latitude = x.Lat,
                    longitude = x.Lng,
                    //timestamp =,
                    type = "gas-station",
                    value = x,
                })
                .ToList();

            await realTimeMap.Geometric.Points.upload(gasStationPoints, newCollection: false, pattern: new RealTimeMap.PointTooltip()
            {
                content = "<strong>${value.Rotulo}</strong>",
                //offset =,
                opacity = 0.8,
                //permanent = true, // default value
            });
        }
    }
}
