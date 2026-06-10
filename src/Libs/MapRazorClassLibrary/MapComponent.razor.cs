using LeafletForBlazor;
using Microsoft.AspNetCore.Components;

namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent
{
    private readonly string[] Colors = ["#007FFF", "#0074EA", "#0069D5", "#005EC0", "#0053AB", "#004896", "#003D81", "#00326C"];

    private RealTimeMap realTimeMap = new();

    [Inject] private GasStationPrices.Services.RoutesService RoutesService { get; set; } = default!;

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

    //protected override Task OnInitializedAsync()
    //{
    //    return base.OnInitializedAsync();
    //}

    public static async Task OnAfterMapLoaded(RealTimeMap.MapEventArgs args) { }

    private static void OnClickMap(RealTimeMap.ClicksMapArgs args) { }

    private static void OnDoubleClickMap(RealTimeMap.ClicksMapArgs args) { }

    public async Task<GasStationPrices.Models.Bounds> SearchRoutesAsync(GasStationPrices.ViewModels.TravelQueryModel model)
    {
        await ClearMapAsync();

        IList<(string NombreRuta, double[][] Coordenadas)> res = await RoutesService.GetRoutesAsync(model, CancellationToken.None);

        for (int i = 0; i < res.Count; i++)
        {
            (string? NombreRuta, double[][]? Coordenadas) = res[i];

            GasStationPrices.Models.GeoJSONLineStringClass inputPolygon = new()
            {
                Type = "Feature",
                Geometry = new GasStationPrices.Models.LineStringGeometry()
                {
                    Type = "Polyline",
                    Coordinates = Coordenadas,
                },
                Properties = new GasStationPrices.Models.Properties()
                {
                    Name = NombreRuta,
                },
            };

            GasStationPrices.Models.GeoJSONPolygonAppearanceClass polygonAppearance = new()
            {
                Data = [inputPolygon],
                Name = NombreRuta,
                Symbology = new GasStationPrices.Models.PolygonSymbol()
                {
                    Color = Colors[i],
                    Opacity = 0.6,
                    Weight = 8,
                },
                Tooltip = new GasStationPrices.Models.Tooltip()
                {
                    Content = NombreRuta,
                    CoordinateInversion = false,
                    Offset = [0, -10],
                    Opacity = 0.9f,
                    Permanent = false,
                    VisibilityZoomLevels = new GasStationPrices.Models.VisibilityZoomLevel()
                    {
                        MaxZoomLevel = 18,
                        MinZoomLevel = 0,
                    },
                }
            };

            await realTimeMap.Geometric.DataFromGeoJSON.addObject(polygonAppearance);
        }

        RealTimeMap.Bounds bounds = new()
        {
            northEast = new RealTimeMap.Location()
            {
                latitude = res.SelectMany(r => r.Coordenadas).Max(c => c[0]),
                longitude = res.SelectMany(r => r.Coordenadas).Max(c => c[1]),
            },
            southWest = new RealTimeMap.Location()
            {
                latitude = res.SelectMany(r => r.Coordenadas).Min(c => c[0]),
                longitude = res.SelectMany(r => r.Coordenadas).Min(c => c[1]),
            },
        };
        realTimeMap.View.setBounds = bounds;

        return new GasStationPrices.Models.Bounds()
        {
            NorthEast = new GasStationPrices.Models.Location()
            {
                Latitude = bounds.northEast.latitude,
                Longitude = bounds.northEast.longitude
            },
            SouthWest = new GasStationPrices.Models.Location()
            {
                Latitude = bounds.southWest.latitude,
                Longitude = bounds.southWest.longitude
            }
        };

        //List<Geography.Models.GeoJSONItem> inputPointsList =
        //[
        //    new Geography.Models.GeoJSONItem()
        //    {
        //        Type = "Feature",
        //        Geometry = new Geography.Models.PointGeometry()
        //        {
        //            Type = "Point",
        //            Coordinates = [43.96898521116147, 25.337392340780355],
        //        },
        //        Properties = new Geography.Models.Properties()
        //        {
        //            Name = "name",
        //        },
        //    },
        //    new Geography.Models.GeoJSONItem()
        //    {
        //        Type = "Feature",
        //        Geometry = new Geography.Models.PointGeometry()
        //        {
        //            Type = "Point",
        //            Coordinates  = [43.97596818245641, 25.33369159513244],

        //        },
        //        Properties = new Geography.Models.Properties()
        //        {
        //            Name = "name",
        //        },
        //    },
        //];

        //List<Geography.Models.GeoJSONLineStringClass> inputPolygonList =
        //[
        //    new Geography.Models.GeoJSONLineStringClass()
        //    {
        //        Type = "Feature",
        //        Geometry = new Geography.Models.LineStringGeometry()
        //        {
        //            Type = "Polyline",
        //            Coordinates =
        //            [
        //                [43.97209871008421, 25.32876177213506],
        //                [43.97200458957660, 25.32911901903800],
        //                [43.97191506364906, 25.32894358371036],
        //                [43.97174519194365, 25.32888935676797],
        //                [43.97186226638604, 25.32880323607917],
        //                [43.97183242466567, 25.32865650950876],
        //             ],
        //        },
        //        Properties = new Geography.Models.Properties()
        //        {
        //            Name = "name",
        //        },
        //    },
        //];

        //Geography.Models.GeoJSONPolygonAppearanceClass polygonsAppearance = new()
        //{
        //    Data = [.. inputPolygonList],
        //    Name = "Polygon",
        //    Symbology = new Geography.Models.PolygonSymbol()
        //    {
        //        Color = "red",
        //        Opacity = 0.6,
        //        Weight = 8,
        //    },
        //};

        //Geography.Models.GeoJSONPointAppearanceClass pointsAppearance = new()
        //{
        //    Data = [.. inputPointsList],
        //    Name = "points",
        //    Symbology = new Geography.Models.PointSymbol()
        //    {
        //        Color = "red",
        //        Radius = 10,
        //    },
        //    Tooltip = new Geography.Models.Tooltip()
        //    {
        //        Content = "Points",
        //        Offset = [2, 2],
        //        Permanent = true,
        //        Opacity = 0.6,
        //        VisibilityZoomLevels = new Geography.Models.VisibilityZoomLevel()
        //        {
        //            MaxZoomLevel = 16,
        //            MinZoomLevel = 14,
        //        },
        //    }
        //};

        //await realTimeMap.Geometric.DataFromGeoJSON.addObject(polygonsAppearance);
        //await realTimeMap.Geometric.DataFromGeoJSON.addObject(pointsAppearance);
    }

    public async Task ClearMapAsync()
    {
        await realTimeMap.Geometric.DataFromGeoJSON.clearMap();

        //realTimeMap.Map.refresh();
    }
}
