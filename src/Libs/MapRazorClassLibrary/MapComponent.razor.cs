using Microsoft.JSInterop;
using Seedysoft.Libs.MapRazorClassLibrary.MapModels;

namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent : IAsyncDisposable
{
    private readonly string[] ColorsForRoutes = ["#007FFF", "#0074EA", "#0069D5", "#005EC0", "#0053AB", "#004896", "#003D81", "#00326C"];

    ///// <summary>
    ///// Gets or sets the height of the <see cref="RealTimeMap" />.
    ///// </summary>
    ///// <remarks>
    ///// Default value is <see langword="null" />.
    ///// </remarks>
    //[Parameter] public string? Height { get; set; } = "calc(100vh - 6rem)";

    ///// <summary>
    ///// Gets or sets the width of the <see cref="RealTimeMap" />.
    ///// </summary>
    ///// <remarks>
    ///// Default value is <see langword="null" />.
    ///// </remarks>
    //[Parameter] public string? Width { get; set; } = "calc(100vw - 18rem)";

    ///// <summary>
    ///// Gets or sets the zoom level of the <see cref="RealTimeMap" />.
    ///// </summary>
    ///// <remarks>
    ///// Default value is 14.
    ///// </remarks>
    //[Parameter] public int Zoom { get; set; } = 18;

    public async Task<string?> LoadRoutesAndGasStationsAsync(
        GasStationPrices.ViewModels.TravelQueryModel model
        , CancellationToken cancellationToken)
    {
        await RemoveAllMarkers();

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
            IReadOnlyList<(string NombreRuta, double[,] Coordenadas)> res
            , CancellationToken cancellationToken)
        {
            for (int i = 0; i < res.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                (string? NombreRuta, double[,]? Coordenadas) = res[i];

                await AddPolyline(arrayPolyline: Coordenadas, color: ColorsForRoutes[i]);
            }
        }

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
                where model.PetroleumProductsSelectedIds.Contains(p.IdProducto)
                let v = gasStations.Select(x => x.GetProdById(p.IdProducto)).Where(x => x.HasValue)
                select new
                {
                    IdP = p.IdProducto,
                    Min = v.Min(),
                    Avg = v.Average(),
                };

            LatLng[] gasStationPoints = [..
                from g in gasStations
                let any = g.AllProducts(model.PetroleumProductsSelectedIds).Any(x => x.Value <= (Products.FirstOrDefault(p => p.IdP == x.IdProducto)?.Avg ?? decimal.Zero))
                let pt = any ? "Cheap" : "Other"
                select new LatLng(lat: g.Lat, lng: g.Lon)
            ];

            await AddMarkers(gasStationPoints);
        }
    }

    protected override void OnInitialized() => ObjRef = DotNetObjectReference.Create(this);

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        if (MapModule == null)
        {
            MapModule = await JsRuntime.InvokeAsync<IJSObjectReference>(
                "import", $"./{Core.Helpers.ContentHelper.ContentPath(typeof(MapComponent))}/js/leafletModule.js");

            await CreateMap();
        }
    }
}
