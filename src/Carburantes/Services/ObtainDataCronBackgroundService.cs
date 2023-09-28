using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Seedysoft.UtilsLib.Extensions;
using System.Net.Http.Json;

namespace Seedysoft.Carburantes.Services;

public class ObtainDataCronBackgroundService : CronBackgroundServiceLib.CronBackgroundService
{
    private static readonly System.Text.Json.JsonSerializerOptions JsonOptions = new() { };

    private readonly IServiceProvider ServiceProvider;
    private readonly ILogger<ObtainDataCronBackgroundService> Logger;
    private readonly Core.Settings.SettingsRoot Settings;
    private readonly HttpClient Client = default!;

    public ObtainDataCronBackgroundService(IServiceProvider serviceProvider)
        : base(serviceProvider.GetRequiredService<IConfiguration>().GetSection(nameof(Core.Settings.SettingsRoot)).Get<Core.Settings.SettingsRoot>()!.ObtainDataSettings)
    {
        ServiceProvider = serviceProvider;
        Settings = serviceProvider.GetRequiredService<IConfiguration>().GetSection(nameof(Core.Settings.SettingsRoot)).Get<Core.Settings.SettingsRoot>()!;

        Logger = ServiceProvider.GetRequiredService<ILogger<ObtainDataCronBackgroundService>>();

        Client = ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(Core.Settings.Minetur));
        Client.BaseAddress = new Uri(Settings.Minetur.Uris.Base);
    }

    protected override async Task DoWorkAsync(CancellationToken cancellationToken) => await MainAsync(cancellationToken);

    public async Task MainAsync(CancellationToken cancellationToken)
    {
        try
        {
            using IServiceScope ServiceScope = ServiceProvider.CreateScope();

            await DeleteAllRecordsAsync(ServiceScope.ServiceProvider.GetRequiredService<Infrastructure.Data.CarburantesDbContext>());

            int Today = int.Parse(DateTime.UtcNow.Date.ToString(UtilsLib.Constants.Formats.YearMonthDayFormat));

            await LoadProductosPetroliferosAsync(ServiceScope.ServiceProvider, Today, cancellationToken);

            Task ComunidadesAutonomasTask = LoadComunidadesAutonomasAsync(ServiceScope.ServiceProvider, Today, cancellationToken);
            Task ProvinciasTask = LoadProvinciasAsync(ServiceScope.ServiceProvider, Today, cancellationToken);
            Task MunicipiosTask = LoadMunicipiosAsync(ServiceScope.ServiceProvider, Today, cancellationToken);
            Task EstacionesServicioTask = LoadEstacionesServicioAsync(ServiceScope.ServiceProvider, Today, cancellationToken);
            Task EstacionProductoPrecioTask = LoadEstacionProductoPrecioAsync(ServiceScope.ServiceProvider, Today, cancellationToken);

            List<Task> AllTasks = new()
            {
                ComunidadesAutonomasTask,
                ProvinciasTask,
                MunicipiosTask,
                EstacionesServicioTask,
                EstacionProductoPrecioTask,
            };

            while (AllTasks.Any())
            {
                Task FinishedTask = await Task.WhenAny(AllTasks);

                _ = AllTasks.Remove(FinishedTask);
            }
        }
        catch (TaskCanceledException e) when (Logger.Handle(e, "Task cancelled.")) { }
        catch (Exception e) when (Logger.Handle(e, "Unhandled exception.")) { }
        finally { await Task.CompletedTask; }
    }

    private async Task DeleteAllRecordsAsync(Infrastructure.Data.CarburantesDbContext dbCtx)
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

            if (!(Resultado?.Any() ?? false))
                return;

            Infrastructure.Data.CarburantesDbContext DbCtx = serviceProvider.GetRequiredService<Infrastructure.Data.CarburantesDbContext>();
            Infrastructure.Data.CarburantesHistDbContext HistDbCtx = serviceProvider.GetRequiredService<Infrastructure.Data.CarburantesHistDbContext>();

            List<Core.Entities.ProductoPetrolifero> NewObjects = new(Resultado.Length);
            List<Core.Entities.ProductoPetroliferoHist> HistObjects = new(Resultado.Length);

            for (int i = 0; i < Resultado.Length; i++)
            {
                int IdProd = int.Parse(Resultado[i].IDProducto);

                NewObjects.Add(new Core.Entities.ProductoPetrolifero()
                {
                    IdProducto = IdProd,
                    NombreProducto = Resultado[i].NombreProducto,
                    NombreProductoAbreviatura = Resultado[i].NombreProductoAbreviatura,
                    AtDate = today
                });
                HistObjects.Add(new Core.Entities.ProductoPetroliferoHist()
                {
                    IdProducto = IdProd,
                    NombreProducto = Resultado[i].NombreProducto,
                    NombreProductoAbreviatura = Resultado[i].NombreProductoAbreviatura,
                    AtDate = today
                });
            }

            await DbCtx.BulkInsertAsync(NewObjects, cancellationToken: cancellationToken);
            await HistDbCtx.BulkInsertOrUpdateAsync(HistObjects, cancellationToken: cancellationToken);
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

            if (!(Resultado?.Any() ?? false))
                return;

            Infrastructure.Data.CarburantesDbContext DbCtx = serviceProvider.GetRequiredService<Infrastructure.Data.CarburantesDbContext>();
            Infrastructure.Data.CarburantesHistDbContext HistDbCtx = serviceProvider.GetRequiredService < Infrastructure.Data.CarburantesHistDbContext >();

            List<Core.Entities.ComunidadAutonoma> NewObjects = new(Resultado.Length);
            List<Core.Entities.ComunidadAutonomaHist> HistObjects = new(Resultado.Length);

            for (int i = 0; i < Resultado.Length; i++)
            {
                int IdCcaa = int.Parse(Resultado[i].IDCCAA);

                NewObjects.Add(new Core.Entities.ComunidadAutonoma()
                {
                    IdComunidadAutonoma = IdCcaa,
                    NombreComunidadAutonoma = Resultado[i].CCAA,
                    AtDate = today
                });
                HistObjects.Add(new Core.Entities.ComunidadAutonomaHist()
                {
                    IdComunidadAutonoma = IdCcaa,
                    NombreComunidadAutonoma = Resultado[i].CCAA,
                    AtDate = today
                });
            }

            await DbCtx.BulkInsertAsync(NewObjects, cancellationToken: cancellationToken);
            await HistDbCtx.BulkInsertOrUpdateAsync(HistObjects, cancellationToken: cancellationToken);
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

            if (!(Resultado?.Any() ?? false))
                return;

            Infrastructure.Data.CarburantesDbContext DbCtx = serviceProvider.GetRequiredService<Infrastructure.Data.CarburantesDbContext>();
            Infrastructure.Data.CarburantesHistDbContext HistDbCtx = serviceProvider.GetRequiredService < Infrastructure.Data.CarburantesHistDbContext >();

            List<Core.Entities.Provincia> NewObjects = new(Resultado.Length);
            List<Core.Entities.ProvinciaHist> HistObjects = new(Resultado.Length);

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
                    AtDate = today
                });
                HistObjects.Add(new Core.Entities.ProvinciaHist()
                {
                    IdProvincia = IdProv,
                    IdComunidadAutonoma = IdCcaa,
                    NombreProvincia = Resultado[i].Provincia,
                    AtDate = today
                });
            }

            await DbCtx.BulkInsertAsync(NewObjects, cancellationToken: cancellationToken);
            await HistDbCtx.BulkInsertOrUpdateAsync(HistObjects, cancellationToken: cancellationToken);
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

            if (!(Resultado?.Any() ?? false))
                return;

            Infrastructure.Data.CarburantesDbContext DbCtx = serviceProvider.GetRequiredService<Infrastructure.Data.CarburantesDbContext>();
            Infrastructure.Data.CarburantesHistDbContext HistDbCtx = serviceProvider.GetRequiredService < Infrastructure.Data.CarburantesHistDbContext >();

            List<Core.Entities.Municipio> NewObjects = new(Resultado.Length);
            List<Core.Entities.MunicipioHist> HistObjects = new(Resultado.Length);

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
                    AtDate = today
                });
                HistObjects.Add(new Core.Entities.MunicipioHist()
                {
                    IdMunicipio = IdMun,
                    IdProvincia = IdProv,
                    NombreMunicipio = Resultado[i].Municipio,
                    AtDate = today
                });
            }

            await DbCtx.BulkInsertAsync(NewObjects, cancellationToken: cancellationToken);
            await HistDbCtx.BulkInsertOrUpdateAsync(HistObjects, cancellationToken: cancellationToken);
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

            if (!(Resultado?.Estaciones.Any() ?? false))
                return;

            Infrastructure.Data.CarburantesDbContext DbCtx = serviceProvider.GetRequiredService<Infrastructure.Data.CarburantesDbContext>();
            Infrastructure.Data.CarburantesHistDbContext HistDbCtx = serviceProvider.GetRequiredService < Infrastructure.Data.CarburantesHistDbContext >();

            List<Core.Entities.EstacionServicio> NewObjects = new(Resultado.Estaciones.Length);
            List<Core.Entities.EstacionServicioHist> HistObjects = new(Resultado.Estaciones.Length);

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
                    AtDate = today
                });
                HistObjects.Add(new Core.Entities.EstacionServicioHist()
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

            await DbCtx.BulkInsertAsync(NewObjects, cancellationToken: cancellationToken);
            await HistDbCtx.BulkInsertOrUpdateAsync(HistObjects, cancellationToken: cancellationToken);
        }
        catch (Exception e) when (Logger.Handle(e, $"Error on '{OrigenRegistros}' failed.")) { }
    }

    private async Task LoadEstacionProductoPrecioAsync(IServiceProvider serviceProvider, int today, CancellationToken cancellationToken)
    {
        const string OrigenRegistros = "EstacionServicioProducto";

        try
        {
            Infrastructure.Data.CarburantesDbContext DbCtx = serviceProvider.GetRequiredService<Infrastructure.Data.CarburantesDbContext>();
            Infrastructure.Data.CarburantesHistDbContext HistDbCtx = serviceProvider.GetRequiredService < Infrastructure.Data.CarburantesHistDbContext >();

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

                if (!(Resultado?.EstacionServicioProducto.Any() ?? false))
                    continue;

                List<Core.Entities.EstacionProductoPrecio> NewObjects = new(Resultado.EstacionServicioProducto.Length);
                List<Core.Entities.EstacionProductoPrecioHist> HistObjects = new(Resultado.EstacionServicioProducto.Length);

                for (int j = 0; j < Resultado.EstacionServicioProducto.Length; j++)
                {
                    Core.JsonObjects.Minetur.EstacionServicioProductoJson EstacionServicioProducto = Resultado.EstacionServicioProducto[j];
                    int EstacionServicioId = int.Parse(EstacionServicioProducto.IDEESS);
                    int NuevoPrecio = (int)(decimal.Parse(EstacionServicioProducto.PrecioProducto, UtilsLib.Constants.Formats.ESCultureInfo) * 1_000);

                    NewObjects.Add(new Core.Entities.EstacionProductoPrecio()
                    {
                        IdEstacion = EstacionServicioId,
                        IdProducto = ProductoPetroliferoId,
                        CentimosDeEuro = NuevoPrecio,
                        AtDate = today
                    });
                    HistObjects.Add(new Core.Entities.EstacionProductoPrecioHist()
                    {
                        IdEstacion = EstacionServicioId,
                        IdProducto = ProductoPetroliferoId,
                        CentimosDeEuro = NuevoPrecio,
                        AtDate = today
                    });
                }

                await DbCtx.BulkInsertAsync(NewObjects, cancellationToken: cancellationToken);
                await HistDbCtx.BulkInsertOrUpdateAsync(HistObjects, cancellationToken: cancellationToken);
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
}
