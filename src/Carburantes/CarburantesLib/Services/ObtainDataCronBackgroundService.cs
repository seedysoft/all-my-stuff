using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using Seedysoft.Carburantes.CoreLib.ViewModels;
using Seedysoft.CarburantesLib.Infrastructure.Data;
using Seedysoft.InfrastructureLib.Extensions;
using Seedysoft.UtilsLib.Extensions;
using System.Net.Http.Json;

namespace Seedysoft.CarburantesLib.Services;

public sealed class ObtainDataCronBackgroundService : CronBackgroundServiceLib.CronBackgroundService
{
    private const string ConnectionStringName = nameof(CarburantesDbContext);

    private static readonly System.Text.Json.JsonSerializerOptions JsonOptions = new() { };

    private readonly IServiceProvider ServiceProvider;
    private readonly ILogger<ObtainDataCronBackgroundService> Logger;
    private readonly Carburantes.CoreLib.Settings.SettingsRoot Settings;
    private readonly HttpClient Client = default!;

    public ObtainDataCronBackgroundService(IServiceProvider serviceProvider)
        : base(serviceProvider.GetRequiredService<IConfiguration>().GetSection(nameof(Carburantes.CoreLib.Settings.SettingsRoot)).Get<Carburantes.CoreLib.Settings.SettingsRoot>()!.ObtainDataSettings)
    {
        ServiceProvider = serviceProvider;
        Settings = serviceProvider.GetRequiredService<IConfiguration>().GetSection(nameof(Carburantes.CoreLib.Settings.SettingsRoot)).Get<Carburantes.CoreLib.Settings.SettingsRoot>()!;

        Logger = ServiceProvider.GetRequiredService<ILogger<ObtainDataCronBackgroundService>>();

        Client = ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(Carburantes.CoreLib.Settings.Minetur));
        Client.BaseAddress = new Uri(Settings.Minetur.Uris.Base);
    }

    private CarburantesDbContext GetContext(int fullDay)
    {
        const int FirstObtained = 230712;
        ArgumentOutOfRangeException.ThrowIfLessThan(fullDay, FirstObtained);

        string YearMonth = fullDay.ToString()[..4];

        string ConnectionString = ServiceProvider.GetRequiredService<IConfiguration>().GetConnectionString($"{ConnectionStringName}") 
            ?? throw new KeyNotFoundException($"Connection string '{ConnectionStringName}' not found.");

        string FullFilePath = Path.GetFullPath(
            ConnectionString.Replace("{yyMM}", YearMonth),
            System.Reflection.Assembly.GetExecutingAssembly().Location);

        DbContextOptions<CarburantesDbContext> dbContextOptions = new();
        DbContextOptionsBuilder dbContextOptionsBuilder = new(dbContextOptions);
        _ = dbContextOptionsBuilder.UseSqlite($"{UtilsLib.Constants.DatabaseStrings.DataSource}{FullFilePath}");
        dbContextOptionsBuilder.ConfigureDebugOptions();
        dbContextOptions = (DbContextOptions<CarburantesDbContext>)dbContextOptionsBuilder.Options;

        CarburantesDbContext carburantesDbContext = new(dbContextOptions);

        carburantesDbContext.Database.Migrate();

        return carburantesDbContext;
    }

    public override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        try
        {
            int Today = int.Parse(DateTime.UtcNow.ToString(UtilsLib.Constants.Formats.YearMonthDayFormat));

            await DeleteAllRecordsAsync(GetContext(Today), Today);

            await LoadProductosPetroliferosAsync(Today, cancellationToken);

            Task ComunidadesAutonomasTask = LoadComunidadesAutonomasAsync(Today, cancellationToken);
            Task ProvinciasTask = LoadProvinciasAsync(Today, cancellationToken);
            Task MunicipiosTask = LoadMunicipiosAsync(Today, cancellationToken);
            Task EstacionesServicioTask = LoadEstacionesServicioAsync(Today, cancellationToken);
            Task EstacionProductoPrecioTask = LoadEstacionProductoPrecioAsync(Today, cancellationToken);

            List<Task> AllTasks =
            [
                ComunidadesAutonomasTask,
                ProvinciasTask,
                MunicipiosTask,
                EstacionesServicioTask,
                EstacionProductoPrecioTask,
            ];

            while (AllTasks.Count != 0)
            {
                Task FinishedTask = await Task.WhenAny(AllTasks);

                _ = AllTasks.Remove(FinishedTask);
            }
        }
        catch (TaskCanceledException e) when (Logger.Handle(e, "Task cancelled.")) { }
        catch (Exception e) when (Logger.Handle(e, "Unhandled exception.")) { }
        finally { await Task.CompletedTask; }
    }

    private async Task DeleteAllRecordsAsync(CarburantesDbContext dbCtx, int day)
    {
        try
        {
            await dbCtx.BulkDeleteAsync(dbCtx.ProductosPetroliferos.Where(x => x.AtDate == day));
            await dbCtx.BulkDeleteAsync(dbCtx.ComunidadesAutonomas.Where(x => x.AtDate == day));
            await dbCtx.BulkDeleteAsync(dbCtx.Provincias.Where(x => x.AtDate == day));
            await dbCtx.BulkDeleteAsync(dbCtx.Municipios.Where(x => x.AtDate == day));
            await dbCtx.BulkDeleteAsync(dbCtx.EstacionesServicio.Where(x => x.AtDate == day));
            await dbCtx.BulkDeleteAsync(dbCtx.EstacionProductoPrecios.Where(x => x.AtDate == day));
        }
        catch (TaskCanceledException e) when (Logger.Handle(e, "Task cancelled.")) { }
        catch (Exception e) when (Logger.Handle(e, "Unhandled exception.")) { }
    }

    private async Task LoadProductosPetroliferosAsync(int today, CancellationToken cancellationToken)
    {
        const string OrigenRegistros = "ProductosPetroliferos";

        try
        {
            Carburantes.CoreLib.JsonObjects.Minetur.ProductoPetroliferoJson[]? Resultado =
                await LoadJsonAsync<Carburantes.CoreLib.JsonObjects.Minetur.ProductoPetroliferoJson[]>(
                    string.Format(Settings.Minetur.Uris.ListadosBase, OrigenRegistros),
                    cancellationToken);

            if (!(Resultado?.Length > 0))
                return;

            CarburantesDbContext carburantesDbContext = GetContext(today);

            Carburantes.CoreLib.Entities.ProductoPetrolifero[] Existing = await carburantesDbContext.ProductosPetroliferos
                .Where(x => x.AtDate == today)
                .ToArrayAsync(cancellationToken);

            Carburantes.CoreLib.Entities.ProductoPetrolifero[] Obtained = Resultado
                .Select(x => new Carburantes.CoreLib.Entities.ProductoPetrolifero()
                {
                    IdProducto = int.Parse(x.IDProducto),
                    NombreProducto = x.NombreProducto,
                    NombreProductoAbreviatura = x.NombreProductoAbreviatura,
                    AtDate = today
                })
                .ToArray();

            await carburantesDbContext.BulkDeleteAsync(Existing.Except(Obtained, new Carburantes.CoreLib.Entities.ProductoPetroliferoEqualityComparer()), cancellationToken: cancellationToken);

            await carburantesDbContext.BulkInsertOrUpdateAsync(Obtained, cancellationToken: cancellationToken);
        }
        catch (Exception e) when (Logger.Handle(e, $"Error on '{OrigenRegistros}' failed.")) { }
    }

    private async Task LoadComunidadesAutonomasAsync(int today, CancellationToken cancellationToken)
    {
        const string OrigenRegistros = "ComunidadesAutonomas";

        try
        {
            Carburantes.CoreLib.JsonObjects.Minetur.ComunidadAutonomaJson[]? Resultado = await LoadJsonAsync<Carburantes.CoreLib.JsonObjects.Minetur.ComunidadAutonomaJson[]>(
                string.Format(Settings.Minetur.Uris.ListadosBase, OrigenRegistros),
                cancellationToken);

            if (!(Resultado?.Length > 0))
                return;

            CarburantesDbContext carburantesDbContext = GetContext(today);

            List<Carburantes.CoreLib.Entities.ComunidadAutonoma> NewObjects = new(Resultado.Length);

            for (int i = 0; i < Resultado.Length; i++)
            {
                int IdCcaa = int.Parse(Resultado[i].IDCCAA);

                NewObjects.Add(new Carburantes.CoreLib.Entities.ComunidadAutonoma()
                {
                    IdComunidadAutonoma = IdCcaa,
                    NombreComunidadAutonoma = Resultado[i].CCAA,
                    AtDate = today
                });
            }

            await carburantesDbContext.BulkInsertAsync(NewObjects, cancellationToken: cancellationToken);
        }
        catch (Exception e) when (Logger.Handle(e, $"Error on '{OrigenRegistros}' failed.")) { }
    }

    private async Task LoadProvinciasAsync(int today, CancellationToken cancellationToken)
    {
        const string OrigenRegistros = "Provincias";

        try
        {
            Carburantes.CoreLib.JsonObjects.Minetur.ProvinciaJson[]? Resultado = await LoadJsonAsync<Carburantes.CoreLib.JsonObjects.Minetur.ProvinciaJson[]>(
                string.Format(Settings.Minetur.Uris.ListadosBase, OrigenRegistros),
                cancellationToken);

            if (!(Resultado?.Length > 0))
                return;

            CarburantesDbContext carburantesDbContext = GetContext(today);

            List<Carburantes.CoreLib.Entities.Provincia> NewObjects = new(Resultado.Length);

            for (int i = 0; i < Resultado.Length; i++)
            {
                int IdProv = int.Parse(Resultado[i].IDPovincia);
                int IdCcaa = int.Parse(Resultado[i].IDCCAA);

                NewObjects.Add(new Carburantes.CoreLib.Entities.Provincia()
                {
                    IdProvincia = IdProv,
                    IdComunidadAutonoma = IdCcaa,
                    NombreProvincia = Resultado[i].Provincia,
                    AtDate = today
                });
            }

            await carburantesDbContext.BulkInsertAsync(NewObjects, cancellationToken: cancellationToken);
        }
        catch (Exception e) when (Logger.Handle(e, $"Error on '{OrigenRegistros}' failed.")) { }
    }

    private async Task LoadMunicipiosAsync(int today, CancellationToken cancellationToken)
    {
        const string OrigenRegistros = "Municipios";

        try
        {
            Carburantes.CoreLib.JsonObjects.Minetur.MunicipioJson[]? Resultado = await LoadJsonAsync<Carburantes.CoreLib.JsonObjects.Minetur.MunicipioJson[]>(
                string.Format(Settings.Minetur.Uris.ListadosBase, OrigenRegistros),
                cancellationToken);

            if (!(Resultado?.Length > 0))
                return;

            CarburantesDbContext carburantesDbContext = GetContext(today);

            List<Carburantes.CoreLib.Entities.Municipio> NewObjects = new(Resultado.Length);

            DateTimeOffset AhoraUtc = DateTimeOffset.UtcNow;

            for (int i = 0; i < Resultado.Length; i++)
            {
                int IdMun = int.Parse(Resultado[i].IDMunicipio);
                int IdProv = int.Parse(Resultado[i].IDProvincia);

                NewObjects.Add(new Carburantes.CoreLib.Entities.Municipio()
                {
                    IdMunicipio = IdMun,
                    IdProvincia = IdProv,
                    NombreMunicipio = Resultado[i].Municipio,
                    AtDate = today
                });
            }

            await carburantesDbContext.BulkInsertAsync(NewObjects, cancellationToken: cancellationToken);
        }
        catch (Exception e) when (Logger.Handle(e, $"Error on '{OrigenRegistros}' failed.")) { }
    }

    private async Task LoadEstacionesServicioAsync(int today, CancellationToken cancellationToken)
    {
        const string OrigenRegistros = "EstacionesServicio";

        try
        {
            Carburantes.CoreLib.JsonObjects.Minetur.EstacionesServicioRootJson? Resultado = await LoadJsonAsync<Carburantes.CoreLib.JsonObjects.Minetur.EstacionesServicioRootJson>(
                Settings.Minetur.Uris.EstacionesTerrestres,
                cancellationToken);

            if (!(Resultado?.Estaciones.Length > 0))
                return;

            CarburantesDbContext carburantesDbContext = GetContext(today);

            List<Carburantes.CoreLib.Entities.EstacionServicio> NewObjects = new(Resultado.Estaciones.Length);

            DateTimeOffset AhoraUtc = DateTimeOffset.UtcNow;

            for (int i = 0; i < Resultado.Estaciones.Length; i++)
            {
                Carburantes.CoreLib.JsonObjects.Minetur.EstacionesServicioJson EstacionServicioJson = Resultado.Estaciones[i];
                int IdEs = int.Parse(EstacionServicioJson.IDEESS);
                int IdMunicipio = int.Parse(EstacionServicioJson.IDMunicipio);

                NewObjects.Add(new Carburantes.CoreLib.Entities.EstacionServicio()
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
                    AtDate = today
                });
            }

            await carburantesDbContext.BulkInsertOrUpdateAsync(NewObjects, cancellationToken: cancellationToken);
        }
        catch (Exception e) when (Logger.Handle(e, $"Error on '{OrigenRegistros}' failed.")) { }
    }

    private async Task LoadEstacionProductoPrecioAsync(int today, CancellationToken cancellationToken)
    {
        const string OrigenRegistros = "EstacionServicioProducto";

        try
        {
            CarburantesDbContext DbCtx = GetContext(today);

            int[] ProductoPetroliferoIds = await DbCtx.ProductosPetroliferos
                .Select(x => x.IdProducto)
                .OrderBy(x => x)
                .ToArrayAsync(cancellationToken);

            DateTimeOffset AhoraUtc = DateTimeOffset.UtcNow;

            for (int i = 0; i < ProductoPetroliferoIds.Length; i++)
            {
                int ProductoPetroliferoId = ProductoPetroliferoIds[i];

                Carburantes.CoreLib.JsonObjects.Minetur.EstacionServicioProductoRootJson? Resultado = await LoadJsonAsync<Carburantes.CoreLib.JsonObjects.Minetur.EstacionServicioProductoRootJson>(
                    string.Concat(Settings.Minetur.Uris.EstacionesTerrestres, string.Format(Settings.Minetur.Uris.EstacionesTerrestresFiltroProducto, ProductoPetroliferoId)),
                    cancellationToken);

                if (!(Resultado?.EstacionServicioProducto.Length > 0))
                    continue;

                List<Carburantes.CoreLib.Entities.EstacionProductoPrecio> NewObjects = new(Resultado.EstacionServicioProducto.Length);

                for (int j = 0; j < Resultado.EstacionServicioProducto.Length; j++)
                {
                    Carburantes.CoreLib.JsonObjects.Minetur.EstacionServicioProductoJson EstacionServicioProducto = Resultado.EstacionServicioProducto[j];
                    int EstacionServicioId = int.Parse(EstacionServicioProducto.IDEESS);
                    int NuevoPrecio = (int)(decimal.Parse(EstacionServicioProducto.PrecioProducto, UtilsLib.Constants.Formats.ESCultureInfo) * 1_000);

                    NewObjects.Add(new Carburantes.CoreLib.Entities.EstacionProductoPrecio()
                    {
                        IdEstacion = EstacionServicioId,
                        IdProducto = ProductoPetroliferoId,
                        CentimosDeEuro = NuevoPrecio,
                        AtDate = today
                    });
                }

                await DbCtx.BulkInsertAsync(NewObjects, cancellationToken: cancellationToken);
            }
        }
        catch (Exception e) when (Logger.Handle(e, $"Error on '{OrigenRegistros}' failed.")) { }
    }

    private async Task<TJson?> LoadJsonAsync<TJson>(string requestUri, CancellationToken cancellationToken)
    {
        TJson? Resultado = await Client.GetFromJsonAsync<TJson?>(requestUri, JsonOptions, cancellationToken);

        if (Resultado == null)
            Logger.Critical($"La petición de '{requestUri}' ha devuelto null.");

        return await Task.FromResult(Resultado);
    }

    public async Task<GasStationInfoModel[]> GetGasStationInfoAsync(GasStationQueryModel filter)
    {
        // TODO Make Lazy and shared
        GeometryFactory GeomFactWgs84 = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(UtilsLib.Constants.CoordinateSystemCodes.Wgs84);
        GeometryFactory GeomFactEpsg3857 = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(UtilsLib.Constants.CoordinateSystemCodes.Epsg3857);

        CarburantesDbContext carburantesDbContext =
            GetContext(int.Parse(DateTime.UtcNow.ToString(UtilsLib.Constants.Formats.YearMonthDayFormat)));

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
            return [];

        Carburantes.CoreLib.JsonObjects.GoogleMaps.LocationJson[] Locations = filter.Steps
            .SelectMany(x => x.Locations)
            .Distinct()
            .ToArray();
        if (Locations.Length == 0)
            return [];

        RoutePoint[] RoutePointsImmutable = Locations
            .Select(x => new RoutePoint()
            {
                PointEpsg3857 = GeomFactEpsg3857
                    .CreateGeometry(GeomFactWgs84
                        .CreatePoint(new Coordinate(x.LngNotNull, x.LatNotNull))
                        .ProjectTo(UtilsLib.Constants.CoordinateSystemCodes.Epsg3857))
            }).ToArray();
        if (RoutePointsImmutable.Length == 0)
            return [];

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
        IQueryable<Carburantes.CoreLib.Entities.ProductoPetrolifero> TempQuery = carburantesDbContext.ProductosPetroliferos.AsNoTracking();

        if (filter.PetroleumProductsSelectedIds != null && filter.PetroleumProductsSelectedIds.Any())
            TempQuery = TempQuery.Where(x => filter.PetroleumProductsSelectedIds.Contains(x.IdProducto));

        IQueryable<GasStationInfoModel> ToReturnQuery =
            from h in carburantesDbContext.EstacionProductoPrecios.AsNoTracking()
            join e in carburantesDbContext.EstacionesServicio.AsNoTracking() on h.IdEstacion equals e.IdEstacion
            join m in carburantesDbContext.Municipios.AsNoTracking() on e.IdMunicipio equals m.IdMunicipio
            join a in carburantesDbContext.Provincias.AsNoTracking() on m.IdProvincia equals a.IdProvincia
            join p in TempQuery on h.IdProducto equals p.IdProducto
            where NearStationIds.Contains(e.IdEstacion)
            orderby p.NombreProducto/*, h.Precio IQueryable cannot order by decimals*/
            select new GasStationInfoModel()
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

        return await ToReturnQuery.ToArrayAsync();
    }

    public async Task<IdDescRecord[]> GetPetroleumProductsForFilterAsync()
    {
        CarburantesDbContext carburantesDbContext =
            GetContext(int.Parse(DateTime.UtcNow.ToString(UtilsLib.Constants.Formats.YearMonthDayFormat)));

        IQueryable<IdDescRecord> Query =
            from p in carburantesDbContext.ProductosPetroliferos
            orderby p.NombreProducto
            select new IdDescRecord(p.IdProducto, p.NombreProducto);

        return await Query.ToArrayAsync();
    }
}
