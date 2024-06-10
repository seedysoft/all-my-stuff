using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Seedysoft.BlazorWebApp.Client;
using Seedysoft.UtilsLib.Extensions;
using System.Collections.Immutable;

namespace Seedysoft.BlazorWebApp.Server.Controllers;

[Route(ControllerUris.RoutesControllerUri)]
public sealed class RoutesController : ApiControllerBase
{
    public RoutesController(ILogger<RoutesController> logger) : base(logger) => Logger = logger;

    [HttpPost]
    public async Task<IImmutableList<Client.ViewModels.RouteObtainedModel>> ObtainRoutesAsync(
        [AsParameters] Client.ViewModels.RouteQueryModel routeQueryModel,
        [FromServices] IHttpClientFactory httpClientFactory,
        [FromServices] Carburantes.Core.Settings.SettingsRoot settings)
    {
        Carburantes.Core.JsonObjects.GoogleMaps.Directions.DistanceApiRootJson? DistanceApiResult = await
#if false //DEBUG
            System.Text.Json.JsonSerializer.DeserializeAsync<DistanceApiRootJson>(
                System.IO.File.OpenRead(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Distance.json")));
#else
            LoadJsonAsync<Carburantes.Core.JsonObjects.GoogleMaps.Directions.DistanceApiRootJson>(
                httpClientFactory,
                settings.GoogleMapsPlatform.Directions.GetUri(routeQueryModel.Origin, routeQueryModel.Destination, settings.GoogleMapsPlatform.ApiKey));
#endif

        if (DistanceApiResult == null || DistanceApiResult.Status == null)
        {
            Logger.LogError("La petición de ruta a Google Maps ha devuelto null para '{Origin}' y '{Destination}'.", routeQueryModel.Origin, routeQueryModel.Destination);

            return ImmutableArray<Client.ViewModels.RouteObtainedModel>.Empty;
        }

        if (!(DistanceApiResult.Routes?.Length > 0))
        {
            Logger.LogError("La petición de ruta a Google Maps no ha devuelto ningún resultado para '{Origin}' y '{Destination}'.", routeQueryModel.Origin, routeQueryModel.Destination);

            return ImmutableArray<Client.ViewModels.RouteObtainedModel>.Empty;
        }

        var ToReturn = (
            from Route in DistanceApiResult.Routes
            from Leg in Route.Legs
            select new Client.ViewModels.RouteObtainedModel()
            {
                FromPlace = Leg.StartAddress,
                ToPlace = Leg.EndAddress,
                Summary = Route.Summary,
                Duration = Leg.Duration,
                Distance = Leg.Distance,
                Steps = Leg.Steps.Select(Step => new Client.ViewModels.StepObtainedModel()
                {
                    //Instructions = HtmlHelper.ParseHtml(Step.HtmlInstructions),
                    Locations = Step.Polyline.Locations(),
                }).ToArray(),
                GoogleMapsUri = settings.GoogleMapsPlatform.Maps.GetUri(Leg.StartAddress, Leg.EndAddress),
            }).ToImmutableArray();

        return ToReturn;
    }

    [HttpPost]
    public async Task<IImmutableList<Client.ViewModels.GasStationInfoModel>> ObtainGasStationsAsync(
        [FromBody] Client.ViewModels.GasStationQueryModel filter
        , [FromServices] Carburantes.Infrastructure.Data.CarburantesDbContext carburantesDbContext
        , [FromServices] Carburantes.Core.Settings.SettingsRoot settings)
    {
        GeometryFactory GeomFactWgs84 = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(UtilsLib.Constants.CoordinateSystemCodes.Wgs84);
        GeometryFactory GeomFactEpsg3857 = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(UtilsLib.Constants.CoordinateSystemCodes.Epsg3857);

        var AllStations = await carburantesDbContext.EstacionesServicio
            .AsNoTracking()
            .Select(x => new
            {
                x.IdEstacion,
                LocationEpsg3857 = GeomFactEpsg3857
                    .CreateGeometry(GeomFactWgs84
                        .CreatePoint(new Coordinate(x.LngNotNull, x.LatNotNull))
                        .ProjectTo(UtilsLib.Constants.CoordinateSystemCodes.Epsg3857))
            }).ToListAsync();
        if (AllStations.Count == 0)
            return ImmutableArray<Client.ViewModels.GasStationInfoModel>.Empty;

        var Locations = filter.Steps
            .SelectMany(x => x.Locations)
            .Distinct()
            .ToImmutableArray();
        if (!Locations.Any())
            return ImmutableArray<Client.ViewModels.GasStationInfoModel>.Empty;

        var RoutePointsImmutable = Locations
            .Select(x => new Client.ViewModels.RoutePoint()
            {
                PointEpsg3857 = GeomFactEpsg3857
                    .CreateGeometry(GeomFactWgs84
                        .CreatePoint(new Coordinate(x.LngNotNull, x.LatNotNull))
                        .ProjectTo(UtilsLib.Constants.CoordinateSystemCodes.Epsg3857))
            }).ToImmutableArray();
        if (!RoutePointsImmutable.Any())
            return ImmutableArray<Client.ViewModels.GasStationInfoModel>.Empty;

        List<Geometry> FilteredPoints = new(RoutePointsImmutable.Length) { RoutePointsImmutable.First().PointEpsg3857 };
        Geometry LastPointAdded = FilteredPoints.First();
        for (int i = 1; i < RoutePointsImmutable.Length; i++)
        {
            Geometry PointEpsg3857 = RoutePointsImmutable[i].PointEpsg3857;

            if (!PointEpsg3857.IsWithinDistance(LastPointAdded, filter.MaxDistanceInMeters * 0.9))
            {
                FilteredPoints.Add(PointEpsg3857);
                LastPointAdded = PointEpsg3857;
            }
        }

        List<int> NearStationIds = new(AllStations.Count);

        for (int i = 0; i < FilteredPoints.Count; i++)
        {
            Geometry FilteredPoint = FilteredPoints[i];

            NearStationIds.AddRange(AllStations
                .Where(x => x.LocationEpsg3857.IsWithinDistance(FilteredPoint, filter.MaxDistanceInMeters))
                .Select(x => x.IdEstacion));

            _ = AllStations.RemoveAll(x => NearStationIds.Contains(x.IdEstacion));

            if (AllStations.Count == 0)
                break;
        }

        NearStationIds.Sort();
        IQueryable<Carburantes.Core.Entities.ProductoPetrolifero> TempQuery = carburantesDbContext.ProductosPetroliferos.AsNoTracking();

        if (filter.PetroleumProductsSelectedIds != null && filter.PetroleumProductsSelectedIds.Any())
            TempQuery = TempQuery.Where(x => filter.PetroleumProductsSelectedIds.Contains(x.IdProducto));

        IQueryable<Client.ViewModels.GasStationInfoModel> ToReturnQuery =
            from h in carburantesDbContext.EstacionProductoPrecios.AsNoTracking()
            join e in carburantesDbContext.EstacionesServicio.AsNoTracking() on h.IdEstacion equals e.IdEstacion
            join m in carburantesDbContext.Municipios.AsNoTracking() on e.IdMunicipio equals m.IdMunicipio
            join a in carburantesDbContext.Provincias.AsNoTracking() on m.IdProvincia equals a.IdProvincia
            join p in TempQuery on h.IdProducto equals p.IdProducto
            where NearStationIds.Contains(e.IdEstacion)
            orderby p.NombreProducto/*, h.Precio IQueryable cannot order by decimals*/
            select new Client.ViewModels.GasStationInfoModel()
            {
                IdEstacion = e.IdEstacion,
                Estacion = e.Rotulo,
                Latitud = e.Latitud,
                Longitud = e.LongitudWgs84,
                // GoogleMapsUri se establece en GasStationInfoModel
                //GoogleMapsUri = settings.GoogleMapsPlatform.Places.GetUri(e.Latitud, e.LongitudWgs84, settings.GoogleMapsPlatform.ApiKey),
                Localidad = e.Localidad,
                Municipio = m.NombreMunicipio,
                Precio = h.Euros,
                Producto = p.NombreProducto,
                Provincia = a.NombreProvincia,
            };

        return ImmutableArray.Create(await ToReturnQuery.ToArrayAsync());
    }

    private async Task<TJson?> LoadJsonAsync<TJson>(
        IHttpClientFactory httpClientFactory,
        string requestUri,
        CancellationToken cancellationToken = default)
    {
        HttpClient WebClient = httpClientFactory.CreateClient("Default");

        TJson? Resultado = await WebClient.GetFromJsonAsync<TJson>(requestUri, JsonOptions, cancellationToken);

        if (Resultado == null)
            Logger.LogError("La petición de {RequestUri} ha devuelto null.", requestUri);

        return await Task.FromResult(Resultado);
    }
}
