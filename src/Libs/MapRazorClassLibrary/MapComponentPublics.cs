namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent
{
    public GasStationPrices.Models.ProductLimits[] Prices { get; private set; } =
        [.. GasStationPrices.Models.Minetur.ProductoPetrolifero.All.Select(static p => new GasStationPrices.Models.ProductLimits(p.IdProducto))];

    public async Task<string?> LoadRoutesAsync(
        Travel.ViewModels.TravelQueryModel model,
        CancellationToken cancellationToken)
    {
        string? returnText = null;

        await ShowLoaderAsync();

        await RemoveRoutesAsync();

        Array.ForEach(Prices, x => x.SetPrices(null));

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

        if (string.IsNullOrEmpty(returnText))
        {
            if (res.Count == 0)
            {
                returnText = "⚠️ No route to load ⚠️";
            }
            else
            {
                for (int i = 0; i < res.Count; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    (string? NombreRuta, double[,]? Coordenadas) = res[i];

                    await AddPolylineAsync(arrayPolyline: Coordenadas, color: ColorsForRoutes[i]);
                }
            }
        }

        await HideLoaderAsync();

        return returnText;
    }

    public async Task<string?> LoadGasStationsAsync(
        MapModels.Basic.LatLngBounds latLngBounds,
        IEnumerable<GasStationPrices.Constants.ProductoPetroliferoId>? petroleumProductsSelectedIds,
        CancellationToken cancellationToken)
    {
        string? returnText = null;

        await ShowLoaderAsync();

        //await RemoveGasStationsAsync();

        Array.ForEach(Prices, x => x.SetPrices(null));

        IReadOnlyList<GasStationPrices.ViewModels.GasStationModel> gasStations = await
            GasStationPricesService.GetNearGasStationsAsync(MapModels.Basic.LatLngBounds.Copy(latLngBounds), cancellationToken);
        if (gasStations.Count == 0)
        {
            returnText = "⚠️ No Gas stations loaded ⚠️";
        }
        else
        {
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

            returnText = productLimits.Length == 0
                ? "⚠️ No Products to show ⚠️"
                : await LoadGasStationDataIntoMapAsync(
                    gasStations,
                    petroleumProductsSelectedIds,
                    cancellationToken);
        }

        await HideLoaderAsync();

        return returnText;

        async Task<string?> LoadGasStationDataIntoMapAsync(
            IReadOnlyList<GasStationPrices.ViewModels.GasStationModel> gasStations,
            IEnumerable<GasStationPrices.Constants.ProductoPetroliferoId>? petroleumProductsSelectedIds,
            CancellationToken cancellationToken)
        {
            for (int i = 0; i < gasStations.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    return "Cancellation Requested";

                GasStationPrices.ViewModels.GasStationModel GasStation = gasStations[i];

                IReadOnlyList<(GasStationPrices.Constants.ProductoPetroliferoId IdProducto, decimal Value)> GasStationProducts =
                    GasStation.AllProducts(petroleumProductsSelectedIds);

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

                await AddOrUpdateCircleMarkerAsync(circleMarker, /*popup*/ null, tooltip);
            }

            return null;
        }
    }
}
