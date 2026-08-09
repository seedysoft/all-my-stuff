using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Seedysoft.Libs.MapRazorClassLibrary.Extensions;

namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent
{
    private DotNetObjectReference<MapComponent> ObjRef = default!;
    private IJSObjectReference? MapModule { get; set; }

    private readonly string MapId = $"map-{Guid.NewGuid()}";

    private bool IsMapReady = false;

    private readonly string LeafletJavascriptFile =
        "_content/Seedysoft.Libs.MapRazorClassLibrary/lib/leaflet/leaflet" +
#if DEBUG
        "-src" +
#endif
       ".js";

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
    private readonly string[] ColorsForRoutes =
        ["#007FFF", "#0074EA", "#0069D5", "#005EC0", "#0053AB", "#004896", "#003D81", "#00326C"];

    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    [Inject] private GasStationPrices.Services.GasStationPricesService GasStationPricesService { get; set; } = default!;
    [Inject] private Travel.Services.Routing.RoutingService RoutingService { get; set; } = default!;

    private async Task CreateMapAsync()
    {
        try
        {
            if (MapModule == null)
                return;

            await MapModule.InvokeVoidAsync("createMap", MapId, Center, Zoom, ObjRef);

            if (OnMapCreatedAsyncEventCallback.HasDelegate)
                await OnMapCreatedAsyncEventCallback.InvokeAsync(this);

            IsMapReady = true;

            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await Console.Out.WriteAsync(ex.Message);
        }
    }

    private async Task AddPolylineAsync(double[,] arrayPolyline, string color)
    {
        if (MapModule != null)
        {
            if (arrayPolyline.GetLength(1) != 2)
                throw new ArgumentException($"The {nameof(arrayPolyline)} must be a 2D array with two columns for latitude and longitude.");

            IEnumerable<MapModels.Basic.LatLng> values =
                from i in Enumerable.Range(0, arrayPolyline.GetLength(0))
                select new MapModels.Basic.LatLng(lat: arrayPolyline[i, 0], lng: arrayPolyline[i, 1]);

            MapModels.VectorLayers.Polyline polyline = new([.. values])
            {
                Color = color,
                Weight = 10,
            };

            await MapModule.InvokeVoidAsync("addPolyline", polyline);
        }
    }

    //private async Task AddMarkerAsync(
    //    MapModels.UILayers.Marker? marker = default
    //    , MapModels.Basic.Icon? icon = default
    //    , string? popupContent = default) => await MapModule.InvokeVoidAsync("addOrUpdateMarker", marker, icon, popupContent);

    private async Task AddOrUpdateCircleMarkerAsync(
        MapModels.VectorLayers.CircleMarker circleMarker,
        OneOf.OneOf<string, MapModels.UILayers.Popup>? popup,
        OneOf.OneOf<string, MapModels.UILayers.Tooltip>? tooltip)
    {
        if (MapModule != null)
            await MapModule.InvokeVoidAsync($"addOrUpdateCircleMarker", circleMarker, popup?.Value, tooltip?.Value);
    }

    private async Task RemoveGasStationsAsync()
    {
        if (MapModule != null)
            await MapModule.InvokeVoidAsync("removeGasStations");
    }
    private async Task RemoveRoutesAsync()
    {
        if (MapModule != null)
            await MapModule.InvokeVoidAsync("removeRoutes");
    }

    // Unused. Uncomment if ncccessary
    //private async Task SetViewAsync(LatLng latLng, int zoom)
    //    => await MapModule.InvokeVoidAsync("setView", latLng, zoom);

    private async Task ShowLoaderAsync()
    {
        if (MapModule != null)
            await MapModule.InvokeVoidAsync("showLoader");
    }
    private async Task HideLoaderAsync()
    {
        if (MapModule != null)
            await MapModule.InvokeVoidAsync("hideLoader");
    }

    private async Task ShowGasStationPopupAsync(MapModels.Basic.LatLng args)
    {
        if (MapModule != null)
        {
            GasStationPrices.ViewModels.GasStationModel? GasStation =
                await GasStationPricesService.GetGasStationAsync(args.ToLocation(), default);
            if (GasStation != null)
                await MapModule.InvokeVoidAsync("showGasStationPopup", args, BuildPopupContent(GasStation));
        }

        string BuildPopupContent(GasStationPrices.ViewModels.GasStationModel GasStation)
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
                GasStationPrices.Models.ProductLimits? productLimits =
                    Prices.FirstOrDefault(p => p.IdP == productoPetrolifero.IdProducto);
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
