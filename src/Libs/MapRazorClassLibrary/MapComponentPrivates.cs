using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Seedysoft.Libs.Core.Extensions;
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

    [Inject] private MudBlazor.ISnackbar Snackbar { get; set; } = default!;

    [Inject] private ILogger<MapComponent> Logger { get; set; } = default!;

    [Inject] private GasStationPrices.Services.GasStationPricesService GasStationPricesService { get; set; } = default!;
    [Inject] private Travel.Services.Routing.RoutingService RoutingService { get; set; } = default!;

    private readonly Travel.ViewModels.TravelQueryModelFluentValidator TravelQueryModelFluentValidator = new();

    private GasStationPrices.ViewModels.GasStationsQueryModel GasStationsQueryModel { get; set; } = GasStationPrices.ViewModels.GasStationsQueryModel.
#if DEBUG
        CreateDefault();
#else
        CreateEmpty();
#endif

    private MapModels.Basic.LatLngBounds CurrentLatLngBounds = MapModels.Basic.LatLngBounds.Empty;

    private GasStationPrices.Models.ProductLimits[] Prices { get; set; } =
        [.. GasStationPrices.Models.Minetur.ProductoPetrolifero.All.Select(static p => new GasStationPrices.Models.ProductLimits(p.IdProducto))];

    private void SetPetroleumProductsSelectedIds(
        System.Collections.Immutable.ImmutableSortedSet<GasStationPrices.Models.Minetur.ProductoPetrolifero>? fromWhat)
        => GasStationsQueryModel.PetroleumProductsSelectedIds = [.. fromWhat?.Select(static x => x.IdProducto) ?? []];

    private async Task<IReadOnlyList<GasStationPrices.ViewModels.GasStationModel>> LoadGasStationsAsync(CancellationToken cancellationToken)
    {
        await ShowLoaderAsync();

        IReadOnlyList<GasStationPrices.ViewModels.GasStationModel> gasStations = await
            GasStationPricesService.GetNearGasStationsAsync(CurrentLatLngBounds.ToBounds(), cancellationToken);

        if (gasStations.Count == 0)
        {
            await HideLoaderAsync();
            return [];
        }

        await LoadGasStationDataIntoMapAsync(gasStations, cancellationToken);

        await HideLoaderAsync();

        return gasStations;

        async Task LoadGasStationDataIntoMapAsync(
            IReadOnlyList<GasStationPrices.ViewModels.GasStationModel> gasStations,
            CancellationToken cancellationToken)
        {
            for (int i = 0; i < gasStations.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                GasStationPrices.ViewModels.GasStationModel GasStation = gasStations[i];

                IReadOnlyList<(GasStationPrices.Constants.ProductoPetroliferoId IdProducto, decimal Value)> GasStationProducts =
                    GasStation.AllProducts(GasStationsQueryModel.PetroleumProductsSelectedIds);

                //                          TODO Use colors, sizes, etc...
                MapModels.VectorLayers.CircleMarker circleMarker = new(new MapModels.Basic.LatLng(GasStation.Lat, GasStation.Lon))
                {
                    Fill = true,
                    FillOpacity = 1.0,
                    FillRule = "nonzero",
                };

                string CssClass;
                if (GasStationProducts.Any(x => x.Value == (Prices.FirstOrDefault(p => p.IdP == x.IdProducto)?.Min ?? decimal.Zero)))
                {
                    CssClass = "bgVerde";
                    circleMarker.Color = circleMarker.FillColor = "#00ff00"; // Green = Min
                    circleMarker.Radius = 25;
                }
                else if (GasStationProducts.Any(x => x.Value == (Prices.FirstOrDefault(p => p.IdP == x.IdProducto)?.Max ?? decimal.Zero)))
                {
                    CssClass = "bgRojo";
                    circleMarker.Color = circleMarker.FillColor = "#ff0000"; // Red = Max
                    circleMarker.Radius = 8;
                }
                else if (GasStationProducts.Any(x => x.Value <= (Prices.FirstOrDefault(p => p.IdP == x.IdProducto)?.Avg ?? decimal.Zero)))
                {
                    CssClass = "bgAmarillo";
                    circleMarker.Color = circleMarker.FillColor = "#ffff00"; // Yellow <= Avg
                    circleMarker.Radius = 15;
                }
                else
                {
                    CssClass = "bgNaranja";
                    circleMarker.Color = circleMarker.FillColor = "#ffa500"; // Orange > Avg
                    circleMarker.Radius = 10;
                }

                //MapModels.UILayers.Popup popup = new()
                //{
                //    Content = BuildPopupContent(GasStation, productLimits),
                //};

                MapModels.UILayers.Tooltip tooltip = new()
                {
                    Content = $"<span class='{CssClass}'><b>{GasStation.RotuloTrimed}</b><span>",
                    Direction = MapModels.UILayers.Tooltip.Directions.Top,
                    Permanent = true,
                };

                // TODO                                         Add all markers at same time
                await AddOrUpdateCircleMarkerAsync(circleMarker, /*popup*/ null, tooltip);
            }
        }
    }

    private async Task LoadDataAsync()
    {
        IReadOnlyList<GasStationPrices.ViewModels.GasStationModel> gasStations = await LoadGasStationsAsync(default);
        if (gasStations.Count == 0)
        {
            Array.ForEach(Prices, static x => x.SetPrices(null));
            _ = Snackbar.Add(new MarkupString($"<span>⚠️ No Gas stations loaded ⚠️</span>"), MudBlazor.Severity.Info);
        }
        else
        {
            await LoadProductsPricesAsync(gasStations);
            StateHasChanged();
        }
    }

    private async Task LoadProductsPricesAsync(IReadOnlyList<GasStationPrices.ViewModels.GasStationModel> gasStations)
    {
        await ShowLoaderAsync();

        // For each product, obtain min, average and max
        GasStationPrices.Models.ProductLimits[] productLimits = [..
            from p in GasStationPrices.Models.Minetur.ProductoPetrolifero.All
            //where model.PetroleumProductsSelectedIds.Contains(p.IdProducto)
            let v = gasStations.Select(x => x.GetProdById(p.IdProducto))//.Where(x => x.HasValue)
            select new GasStationPrices.Models.ProductLimits(
                p.IdProducto,
                v?.Min(),
                v?.Average(),
                v?.Max()
            )];

        foreach (GasStationPrices.Models.ProductLimits item in Prices)
            item.SetPrices(productLimits.FirstOrDefault(x => x.IdP == item.IdP));

        await HideLoaderAsync();
    }

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
        catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected error")) { await Console.Out.WriteAsync(e.Message); }
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

    private async Task RemoveRoutesAsync()
    {
        if (MapModule != null)
            await MapModule.InvokeVoidAsync("removeRoutes");
    }

    // Unused. Uncomment if neccessary
    //private async Task SetViewAsync(LatLng latLng, int zoom)
    //    => await MapModule.InvokeVoidAsync("setView", latLng, zoom);
    //private async Task RemoveGasStationsAsync()
    //{
    //    if (MapModule != null)
    //        await MapModule.InvokeVoidAsync("removeGasStations");
    //}

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
                await MapModule.InvokeVoidAsync("showGasStationPopup", BuildPopupContent(GasStation), args);
        }

        string BuildPopupContent(GasStationPrices.ViewModels.GasStationModel GasStation)
        {
            // TODO                                                                 Include a link to Gas Station location
            System.Text.StringBuilder popupContent = new();
            _ = popupContent
                .Append("<div class='container'>")
                .Append($"<p>{GasStation.Localizacion}</p>");

            _ = popupContent
                .Append("<div class='divTable'>")
                .Append("<div class='divTableBody'>");

            foreach (GasStationPrices.Constants.ProductoPetroliferoId item in GasStationsQueryModel.PetroleumProductsSelectedIds)
            {
                GasStationPrices.Models.Minetur.ProductoPetrolifero productoPetrolifero =
                    GasStationPrices.Models.Minetur.ProductoPetrolifero.All.First(x => x.IdProducto == item);

                decimal? GasVal = GasStation.GetProdById(productoPetrolifero.IdProducto);
                if (!GasVal.HasValue)
                    continue;

#pragma warning disable IDE0045 // Convert to conditional expression
                string CssClass;
                GasStationPrices.Models.ProductLimits? productLimits =
                    Prices.FirstOrDefault(p => p.IdP == productoPetrolifero.IdProducto);
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
                    .Append($"<div class='divTableCell'>{productoPetrolifero.Abreviatura.ToUpperInvariant()} - {productoPetrolifero.Nombre} </div>")
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
