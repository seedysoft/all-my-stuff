using LeafletForBlazor;
using Microsoft.AspNetCore.Components;

namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent
{
    private RealTimeMap realTimeMap = new();

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
            latitude = Core.Constants.Earth.Home.Lat,
            longitude = Core.Constants.Earth.Home.Lng,
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

    protected override Task OnInitializedAsync()
    {
        int i = 0;

        return base.OnInitializedAsync();
    }

    private void OnClickMap(RealTimeMap.ClicksMapArgs args)
    {
        if (args.sender != null && realTimeMap == args.sender)
        {
            Console.WriteLine(@"");
        }
    }
}
