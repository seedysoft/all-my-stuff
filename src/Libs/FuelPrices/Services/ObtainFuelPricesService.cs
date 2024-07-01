using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Utils.Extensions;
using System.Collections.Immutable;
using System.Net.Http.Json;

namespace Seedysoft.Libs.FuelPrices.Services;

public sealed class ObtainFuelPricesService : IDisposable
{
    private static readonly System.Text.Json.JsonSerializerOptions JsonOptions = new() { };

    private readonly IServiceProvider ServiceProvider;
    private readonly ILogger<ObtainFuelPricesService> Logger;
    private readonly Core.Settings.SettingsRoot Settings;
    private readonly HttpClient httpClient = default!;

    private readonly Microsoft.Data.Sqlite.SqliteConnection sqliteConnection;
    private readonly DbContextOptions<Infrastructure.Data.FuelPricesDbContext> dbContextOptions;

    public ObtainFuelPricesService(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        Settings = serviceProvider.GetRequiredService<IConfiguration>().GetSection(nameof(Core.Settings.SettingsRoot)).Get<Core.Settings.SettingsRoot>()!;

        Logger = ServiceProvider.GetRequiredService<ILogger<ObtainFuelPricesService>>();

        httpClient = ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(Core.Settings.Minetur));
        httpClient.BaseAddress = new Uri(Settings.Minetur.Uris.Base);

        string ConnectionString = "Data Source=InMemorySample;Mode=Memory;Cache=Shared";
        sqliteConnection = new(ConnectionString);
        sqliteConnection.Open();

        dbContextOptions = new DbContextOptionsBuilder<Infrastructure.Data.FuelPricesDbContext>().UseSqlite(sqliteConnection).Options;

        ServiceProvider.GetRequiredService<Infrastructure.Data.FuelPricesDbContext>().Database.Migrate();
    }

    public async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        try
        {
            using IServiceScope ServiceScope = ServiceProvider.CreateAsyncScope();

            await DeleteAllRecordsAsync(ServiceScope.ServiceProvider.GetRequiredService<Infrastructure.Data.FuelPricesDbContext>());

            int Today = int.Parse(DateTime.UtcNow.Date.ToString(Utils.Constants.Formats.YearMonthDayFormat));

            await LoadProductosPetroliferosAsync(ServiceScope.ServiceProvider, Today, cancellationToken);

            Task ComunidadesAutonomasTask = LoadComunidadesAutonomasAsync(ServiceScope.ServiceProvider, Today, cancellationToken);
            Task ProvinciasTask = LoadProvinciasAsync(ServiceScope.ServiceProvider, Today, cancellationToken);
            Task MunicipiosTask = LoadMunicipiosAsync(ServiceScope.ServiceProvider, Today, cancellationToken);
            Task EstacionesServicioTask = LoadEstacionesServicioAsync(ServiceScope.ServiceProvider, Today, cancellationToken);
            Task EstacionProductoPrecioTask = LoadEstacionProductoPrecioAsync(ServiceScope.ServiceProvider, Today, cancellationToken);

            List<Task> AllTasks =
            [
                ComunidadesAutonomasTask,
                ProvinciasTask,
                MunicipiosTask,
                EstacionesServicioTask,
                EstacionProductoPrecioTask,
            ];

            while (AllTasks.Count > 0)
            {
                Task FinishedTask = await Task.WhenAny(AllTasks);

                _ = AllTasks.Remove(FinishedTask);
            }
        }
        catch (TaskCanceledException e) when (Logger.Handle(e, "Task cancelled.")) { }
        catch (Exception e) when (Logger.Handle(e, "Unhandled exception.")) { }
        finally { await Task.CompletedTask; }
    }

    private async Task DeleteAllRecordsAsync(Infrastructure.Data.FuelPricesDbContext dbCtx)
    {
        try
        {
            await dbCtx.TruncateAsync<Core.Entities.ProductoPetrolifero>();
            await dbCtx.TruncateAsync<Core.Entities.ComunidadAutonoma>();
            await dbCtx.TruncateAsync<Core.Entities.Provincia>();
            await dbCtx.TruncateAsync<Core.Entities.Municipio>();
            await dbCtx.TruncateAsync<Core.Entities.EstacionServicio>();
            await dbCtx.TruncateAsync<Core.Entities.EstacionProductoPrecio>();
        }
        catch (TaskCanceledException e) when (Logger.Handle(e, "Task cancelled.")) { }
        catch (Exception e) when (Logger.Handle(e, "Unhandled exception.")) { }
    }

    private async Task LoadProductosPetroliferosAsync(IServiceProvider serviceProvider, int today, CancellationToken cancellationToken)
    {
        const string OrigenRegistros = "ProductosPetroliferos";

        try
        {
            Core.JsonObjects.Minetur.ProductoPetroliferoJson[]? Resultado = await LoadJsonAsync<Core.JsonObjects.Minetur.ProductoPetroliferoJson[]>(
                string.Format(Settings.Minetur.Uris.ListadosBase, OrigenRegistros),
                cancellationToken);

            if (!(Resultado?.Length > 0))
                return;

            Infrastructure.Data.FuelPricesDbContext DbCtx = serviceProvider.GetRequiredService<Infrastructure.Data.FuelPricesDbContext>();

            List<Core.Entities.ProductoPetrolifero> NewObjects = new(Resultado.Length);

            for (int i = 0; i < Resultado.Length; i++)
            {
                int IdProd = int.Parse(Resultado[i].IDProducto);

                NewObjects.Add(new Core.Entities.ProductoPetrolifero()
                {
                    IdProducto = IdProd,
                    NombreProducto = Resultado[i].NombreProducto,
                    NombreProductoAbreviatura = Resultado[i].NombreProductoAbreviatura,
                    AtDate = today,
                });
            }

            await DbCtx.BulkInsertAsync(NewObjects, cancellationToken: cancellationToken);
        }
        catch (Exception e) when (Logger.Handle(e, $"Error on '{OrigenRegistros}' failed.")) { }
    }

    private async Task LoadComunidadesAutonomasAsync(IServiceProvider serviceProvider, int today, CancellationToken cancellationToken)
    {
        const string OrigenRegistros = "ComunidadesAutonomas";

        try
        {
            Core.JsonObjects.Minetur.ComunidadAutonomaJson[]? Resultado = await LoadJsonAsync<Core.JsonObjects.Minetur.ComunidadAutonomaJson[]>(
                string.Format(Settings.Minetur.Uris.ListadosBase, OrigenRegistros),
                cancellationToken);

            if (!(Resultado?.Length > 0))
                return;

            Infrastructure.Data.FuelPricesDbContext DbCtx = serviceProvider.GetRequiredService<Infrastructure.Data.FuelPricesDbContext>();

            List<Core.Entities.ComunidadAutonoma> NewObjects = new(Resultado.Length);

            for (int i = 0; i < Resultado.Length; i++)
            {
                int IdCcaa = int.Parse(Resultado[i].IDCCAA);

                NewObjects.Add(new Core.Entities.ComunidadAutonoma()
                {
                    IdComunidadAutonoma = IdCcaa,
                    NombreComunidadAutonoma = Resultado[i].CCAA,
                    AtDate = today,
                });
            }

            await DbCtx.BulkInsertAsync(NewObjects, cancellationToken: cancellationToken);
        }
        catch (Exception e) when (Logger.Handle(e, $"Error on '{OrigenRegistros}' failed.")) { }
    }

    private async Task LoadProvinciasAsync(IServiceProvider serviceProvider, int today, CancellationToken cancellationToken)
    {
        const string OrigenRegistros = "Provincias";

        try
        {
            Core.JsonObjects.Minetur.ProvinciaJson[]? Resultado = await LoadJsonAsync<Core.JsonObjects.Minetur.ProvinciaJson[]>(
                string.Format(Settings.Minetur.Uris.ListadosBase, OrigenRegistros),
                cancellationToken);

            if (!(Resultado?.Length > 0))
                return;

            Infrastructure.Data.FuelPricesDbContext DbCtx = serviceProvider.GetRequiredService<Infrastructure.Data.FuelPricesDbContext>();

            List<Core.Entities.Provincia> NewObjects = new(Resultado.Length);

            DateTimeOffset AhoraUtc = DateTimeOffset.UtcNow;

            for (int i = 0; i < Resultado.Length; i++)
            {
                int IdProv = int.Parse(Resultado[i].IDPovincia);
                int IdCcaa = int.Parse(Resultado[i].IDCCAA);

                NewObjects.Add(new Core.Entities.Provincia()
                {
                    IdProvincia = IdProv,
                    IdComunidadAutonoma = IdCcaa,
                    NombreProvincia = Resultado[i].Provincia,
                    AtDate = today,
                });
            }

            await DbCtx.BulkInsertAsync(NewObjects, cancellationToken: cancellationToken);
        }
        catch (Exception e) when (Logger.Handle(e, $"Error on '{OrigenRegistros}' failed.")) { }
    }

    private async Task LoadMunicipiosAsync(IServiceProvider serviceProvider, int today, CancellationToken cancellationToken)
    {
        const string OrigenRegistros = "Municipios";

        try
        {
            Core.JsonObjects.Minetur.MunicipioJson[]? Resultado = await LoadJsonAsync<Core.JsonObjects.Minetur.MunicipioJson[]>(
                string.Format(Settings.Minetur.Uris.ListadosBase, OrigenRegistros),
                cancellationToken);

            if (!(Resultado?.Length > 0))
                return;

            Infrastructure.Data.FuelPricesDbContext DbCtx = serviceProvider.GetRequiredService<Infrastructure.Data.FuelPricesDbContext>();

            List<Core.Entities.Municipio> NewObjects = new(Resultado.Length);

            DateTimeOffset AhoraUtc = DateTimeOffset.UtcNow;

            for (int i = 0; i < Resultado.Length; i++)
            {
                int IdMun = int.Parse(Resultado[i].IDMunicipio);
                int IdProv = int.Parse(Resultado[i].IDProvincia);

                NewObjects.Add(new Core.Entities.Municipio()
                {
                    IdMunicipio = IdMun,
                    IdProvincia = IdProv,
                    NombreMunicipio = Resultado[i].Municipio,
                    AtDate = today,
                });
            }

            await DbCtx.BulkInsertAsync(NewObjects, cancellationToken: cancellationToken);
        }
        catch (Exception e) when (Logger.Handle(e, $"Error on '{OrigenRegistros}' failed.")) { }
    }

    private async Task LoadEstacionesServicioAsync(IServiceProvider serviceProvider, int today, CancellationToken cancellationToken)
    {
        const string OrigenRegistros = "EstacionesServicio";

        try
        {
            Core.JsonObjects.Minetur.EstacionesServicioRootJson? Resultado = await LoadJsonAsync<Core.JsonObjects.Minetur.EstacionesServicioRootJson>(
                Settings.Minetur.Uris.EstacionesTerrestres,
                cancellationToken);

            if (!(Resultado?.Estaciones.Length > 0))
                return;

            Infrastructure.Data.FuelPricesDbContext DbCtx = serviceProvider.GetRequiredService<Infrastructure.Data.FuelPricesDbContext>();

            List<Core.Entities.EstacionServicio> NewObjects = new(Resultado.Estaciones.Length);

            DateTimeOffset AhoraUtc = DateTimeOffset.UtcNow;

            for (int i = 0; i < Resultado.Estaciones.Length; i++)
            {
                Core.JsonObjects.Minetur.EstacionesServicioJson EstacionServicioJson = Resultado.Estaciones[i];
                int IdEs = int.Parse(EstacionServicioJson.IDEESS);
                int IdMunicipio = int.Parse(EstacionServicioJson.IDMunicipio);

                NewObjects.Add(new Core.Entities.EstacionServicio()
                {
                    IdEstacion = IdEs,
                    CodigoPostal = EstacionServicioJson.CodigoPostal,
                    Direccion = EstacionServicioJson.Direccion,
                    Horario = EstacionServicioJson.Horario,
                    IdMunicipio = IdMunicipio,
                    Latitud = EstacionServicioJson.Latitud,
                    LongitudWgs84 = EstacionServicioJson.LongitudWgs84,
                    Localidad = EstacionServicioJson.Localidad,
                    Margen = EstacionServicioJson.Margen,
                    Rotulo = EstacionServicioJson.Rotulo,
                    AtDate = today,
                });
            }

            await DbCtx.BulkInsertAsync(NewObjects, cancellationToken: cancellationToken);
        }
        catch (Exception e) when (Logger.Handle(e, $"Error on '{OrigenRegistros}' failed.")) { }
    }

    private async Task LoadEstacionProductoPrecioAsync(IServiceProvider serviceProvider, int today, CancellationToken cancellationToken)
    {
        const string OrigenRegistros = "EstacionServicioProducto";

        try
        {
            Infrastructure.Data.FuelPricesDbContext DbCtx = serviceProvider.GetRequiredService<Infrastructure.Data.FuelPricesDbContext>();

            int[] ProductoPetroliferoIds = await DbCtx.ProductosPetroliferos
                .Select(x => x.IdProducto)
                .OrderBy(x => x)
                .ToArrayAsync(cancellationToken);

            DateTimeOffset AhoraUtc = DateTimeOffset.UtcNow;

            for (int i = 0; i < ProductoPetroliferoIds.Length; i++)
            {
                int ProductoPetroliferoId = ProductoPetroliferoIds[i];

                Core.JsonObjects.Minetur.EstacionServicioProductoRootJson? Resultado = await LoadJsonAsync<Core.JsonObjects.Minetur.EstacionServicioProductoRootJson>(
                    string.Concat(Settings.Minetur.Uris.EstacionesTerrestres, string.Format(Settings.Minetur.Uris.EstacionesTerrestresFiltroProducto, ProductoPetroliferoId)),
                    cancellationToken);

                if (!(Resultado?.EstacionServicioProducto.Length > 0))
                    continue;

                List<Core.Entities.EstacionProductoPrecio> NewObjects = new(Resultado.EstacionServicioProducto.Length);

                for (int j = 0; j < Resultado.EstacionServicioProducto.Length; j++)
                {
                    Core.JsonObjects.Minetur.EstacionServicioProductoJson EstacionServicioProducto = Resultado.EstacionServicioProducto[j];
                    int EstacionServicioId = int.Parse(EstacionServicioProducto.IDEESS);
                    int NuevoPrecio = (int)(decimal.Parse(EstacionServicioProducto.PrecioProducto, Utils.Constants.Formats.ESCultureInfo) * 1_000);

                    NewObjects.Add(new Core.Entities.EstacionProductoPrecio()
                    {
                        IdEstacion = EstacionServicioId,
                        IdProducto = ProductoPetroliferoId,
                        CentimosDeEuro = NuevoPrecio,
                        AtDate = today,
                    });
                }

                await DbCtx.BulkInsertAsync(NewObjects, cancellationToken: cancellationToken);
            }
        }
        catch (Exception e) when (Logger.Handle(e, $"Error on '{OrigenRegistros}' failed.")) { }
    }

    private async Task<TJson?> LoadJsonAsync<TJson>(string requestUri, CancellationToken cancellationToken)
    {
        TJson? Resultado = await httpClient.GetFromJsonAsync<TJson?>(requestUri, JsonOptions, cancellationToken);

        if (Resultado == null)
            Logger.Critical($"La petición de '{requestUri}' ha devuelto null.");

        return await Task.FromResult(Resultado);
    }

    public async Task<IImmutableList<Core.ViewModels.GasStationInfoModel>> ObtainDataAsync(Core.ViewModels.GasStationQueryModel filter)
    {
        NetTopologySuite.Geometries.GeometryFactory GeomFactWgs84 = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(Libs.Utils.Constants.CoordinateSystemCodes.Wgs84);
        NetTopologySuite.Geometries.GeometryFactory GeomFactEpsg3857 = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(Libs.Utils.Constants.CoordinateSystemCodes.Epsg3857);

        Infrastructure.Data.FuelPricesDbContext fuelPricesDbContext = ServiceProvider.GetRequiredService<Infrastructure.Data.FuelPricesDbContext>();

        var AllStations = await fuelPricesDbContext.EstacionesServicio
            .AsNoTracking()
            .Select(x => new
            {
                x.IdEstacion,
                LocationEpsg3857 = GeomFactEpsg3857
                    .CreateGeometry(GeomFactWgs84
                        .CreatePoint(new NetTopologySuite.Geometries.Coordinate(x.LngNotNull, x.LatNotNull))
                        .ProjectTo(Libs.Utils.Constants.CoordinateSystemCodes.Epsg3857)),
            }).ToListAsync();
        if (AllStations.Count == 0)
            return ImmutableArray<Core.ViewModels.GasStationInfoModel>.Empty;

        var Locations = filter.Steps
            .SelectMany(x => x.Locations)
            .Distinct()
            .ToImmutableArray();
        if (!Locations.Any())
            return ImmutableArray<Core.ViewModels.GasStationInfoModel>.Empty;

        var TravelPointsImmutable = Locations
            .Select(x => new Core.ViewModels.TravelPoint()
            {
                PointEpsg3857 = GeomFactEpsg3857
                    .CreateGeometry(GeomFactWgs84
                        .CreatePoint(new NetTopologySuite.Geometries.Coordinate(x.LngNotNull, x.LatNotNull))
                        .ProjectTo(Libs.Utils.Constants.CoordinateSystemCodes.Epsg3857)),
            }).ToImmutableArray();
        if (!TravelPointsImmutable.Any())
            return ImmutableArray<Core.ViewModels.GasStationInfoModel>.Empty;

        List<NetTopologySuite.Geometries.Geometry> FilteredPoints = new(TravelPointsImmutable.Length) {
            TravelPointsImmutable.First().PointEpsg3857, };
        NetTopologySuite.Geometries.Geometry LastPointAdded = FilteredPoints[0];
        for (int i = 1; i < TravelPointsImmutable.Length; i++)
        {
            NetTopologySuite.Geometries.Geometry PointEpsg3857 = TravelPointsImmutable[i].PointEpsg3857;

            if (!PointEpsg3857.IsWithinDistance(LastPointAdded, filter.MaxDistanceInMeters * 0.9))
            {
                FilteredPoints.Add(PointEpsg3857);
                LastPointAdded = PointEpsg3857;
            }
        }

        List<int> NearStationIds = new(AllStations.Count);

        for (int i = 0; i < FilteredPoints.Count; i++)
        {
            NetTopologySuite.Geometries.Geometry FilteredPoint = FilteredPoints[i];

            NearStationIds.AddRange(AllStations
                .Where(x => x.LocationEpsg3857.IsWithinDistance(FilteredPoint, filter.MaxDistanceInMeters))
                .Select(x => x.IdEstacion));

            _ = AllStations.RemoveAll(x => NearStationIds.Contains(x.IdEstacion));

            if (AllStations.Count == 0)
                break;
        }

        NearStationIds.Sort();
        IQueryable<Core.Entities.ProductoPetrolifero> TempQuery = fuelPricesDbContext.ProductosPetroliferos.AsNoTracking();

        if (filter.PetroleumProductsSelectedIds != null && filter.PetroleumProductsSelectedIds.Any())
            TempQuery = TempQuery.Where(x => filter.PetroleumProductsSelectedIds.Contains(x.IdProducto));

        IQueryable<Core.ViewModels.GasStationInfoModel> ToReturnQuery =
            from h in fuelPricesDbContext.EstacionProductoPrecios.AsNoTracking()
            join e in fuelPricesDbContext.EstacionesServicio.AsNoTracking() on h.IdEstacion equals e.IdEstacion
            join m in fuelPricesDbContext.Municipios.AsNoTracking() on e.IdMunicipio equals m.IdMunicipio
            join a in fuelPricesDbContext.Provincias.AsNoTracking() on m.IdProvincia equals a.IdProvincia
            join p in TempQuery on h.IdProducto equals p.IdProducto
            where NearStationIds.Contains(e.IdEstacion)
            orderby p.NombreProducto/*, h.Precio IQueryable cannot order by decimals*/
            select new Core.ViewModels.GasStationInfoModel()
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

    public async Task<IImmutableList<Core.ViewModels.IdDescRecord>> GetPetroleumProductsAsync()
    {
        Infrastructure.Data.FuelPricesDbContext fuelPricesDbContext = ServiceProvider.GetRequiredService<Infrastructure.Data.FuelPricesDbContext>();

        IQueryable<Core.ViewModels.IdDescRecord> Query =
            from p in fuelPricesDbContext.ProductosPetroliferos
            orderby p.NombreProducto
            select new Core.ViewModels.IdDescRecord(p.IdProducto, p.NombreProducto);

        return ImmutableArray.Create(await Query.ToArrayAsync());
    }

    public void Dispose()
    {
        ((IDisposable)httpClient)?.Dispose();
        ((IDisposable)sqliteConnection)?.Dispose();
    }
}
