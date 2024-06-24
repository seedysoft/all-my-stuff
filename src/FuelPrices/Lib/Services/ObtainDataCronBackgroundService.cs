using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Seedysoft.FuelPrices.Lib.Core.Entities;
using Seedysoft.FuelPrices.Lib.Core.JsonObjects.Minetur;
using Seedysoft.FuelPrices.Lib.Infrastructure.Data;
// TODO  Remove using Seedysoft when Dependencies where well configured. 
using Seedysoft.Libs.Utils.Extensions;
using System.Net.Http.Json;

namespace Seedysoft.FuelPrices.Lib.Services;

public sealed class ObtainDataCronBackgroundService : Libs.CronBackgroundService.CronBackgroundService
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

    public override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        try
        {
            using IServiceScope ServiceScope = ServiceProvider.CreateScope();

            await DeleteAllRecordsAsync(ServiceScope.ServiceProvider.GetRequiredService<FuelPricesDbContext>());

            int Today = int.Parse(DateTime.UtcNow.Date.ToString(Libs.Utils.Constants.Formats.YearMonthDayFormat));

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

    private async Task DeleteAllRecordsAsync(FuelPricesDbContext dbCtx)
    {
        try
        {
            await dbCtx.TruncateAsync<ProductoPetrolifero>();
            await dbCtx.TruncateAsync<ComunidadAutonoma>();
            await dbCtx.TruncateAsync<Provincia>();
            await dbCtx.TruncateAsync<Municipio>();
            await dbCtx.TruncateAsync<EstacionServicio>();
            await dbCtx.TruncateAsync<EstacionProductoPrecio>();
        }
        catch (TaskCanceledException e) when (Logger.Handle(e, "Task cancelled.")) { }
        catch (Exception e) when (Logger.Handle(e, "Unhandled exception.")) { }
    }

    private async Task LoadProductosPetroliferosAsync(IServiceProvider serviceProvider, int today, CancellationToken cancellationToken)
    {
        const string OrigenRegistros = "ProductosPetroliferos";

        try
        {
            ProductoPetroliferoJson[]? Resultado = await LoadJsonAsync<ProductoPetroliferoJson[]>(
                string.Format(Settings.Minetur.Uris.ListadosBase, OrigenRegistros),
                cancellationToken);

            if (!(Resultado?.Length > 0))
                return;

            FuelPricesDbContext DbCtx = serviceProvider.GetRequiredService<FuelPricesDbContext>();

            List<ProductoPetrolifero> NewObjects = new(Resultado.Length);

            for (int i = 0; i < Resultado.Length; i++)
            {
                int IdProd = int.Parse(Resultado[i].IDProducto);

                NewObjects.Add(new ProductoPetrolifero()
                {
                    IdProducto = IdProd,
                    NombreProducto = Resultado[i].NombreProducto,
                    NombreProductoAbreviatura = Resultado[i].NombreProductoAbreviatura,
                    AtDate = today
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
            ComunidadAutonomaJson[]? Resultado = await LoadJsonAsync<ComunidadAutonomaJson[]>(
                string.Format(Settings.Minetur.Uris.ListadosBase, OrigenRegistros),
                cancellationToken);

            if (!(Resultado?.Length > 0))
                return;

            FuelPricesDbContext DbCtx = serviceProvider.GetRequiredService<FuelPricesDbContext>();

            List<ComunidadAutonoma> NewObjects = new(Resultado.Length);

            for (int i = 0; i < Resultado.Length; i++)
            {
                int IdCcaa = int.Parse(Resultado[i].IDCCAA);

                NewObjects.Add(new ComunidadAutonoma()
                {
                    IdComunidadAutonoma = IdCcaa,
                    NombreComunidadAutonoma = Resultado[i].CCAA,
                    AtDate = today
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
            ProvinciaJson[]? Resultado = await LoadJsonAsync<ProvinciaJson[]>(
                string.Format(Settings.Minetur.Uris.ListadosBase, OrigenRegistros),
                cancellationToken);

            if (!(Resultado?.Length > 0))
                return;

            FuelPricesDbContext DbCtx = serviceProvider.GetRequiredService<FuelPricesDbContext>();

            List<Provincia> NewObjects = new(Resultado.Length);

            DateTimeOffset AhoraUtc = DateTimeOffset.UtcNow;

            for (int i = 0; i < Resultado.Length; i++)
            {
                int IdProv = int.Parse(Resultado[i].IDPovincia);
                int IdCcaa = int.Parse(Resultado[i].IDCCAA);

                NewObjects.Add(new Provincia()
                {
                    IdProvincia = IdProv,
                    IdComunidadAutonoma = IdCcaa,
                    NombreProvincia = Resultado[i].Provincia,
                    AtDate = today
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
            MunicipioJson[]? Resultado = await LoadJsonAsync<MunicipioJson[]>(
                string.Format(Settings.Minetur.Uris.ListadosBase, OrigenRegistros),
                cancellationToken);

            if (!(Resultado?.Length > 0))
                return;

            FuelPricesDbContext DbCtx = serviceProvider.GetRequiredService<FuelPricesDbContext>();

            List<Municipio> NewObjects = new(Resultado.Length);

            DateTimeOffset AhoraUtc = DateTimeOffset.UtcNow;

            for (int i = 0; i < Resultado.Length; i++)
            {
                int IdMun = int.Parse(Resultado[i].IDMunicipio);
                int IdProv = int.Parse(Resultado[i].IDProvincia);

                NewObjects.Add(new Municipio()
                {
                    IdMunicipio = IdMun,
                    IdProvincia = IdProv,
                    NombreMunicipio = Resultado[i].Municipio,
                    AtDate = today
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
            EstacionesServicioRootJson? Resultado = await LoadJsonAsync<EstacionesServicioRootJson>(
                Settings.Minetur.Uris.EstacionesTerrestres,
                cancellationToken);

            if (!(Resultado?.Estaciones.Length > 0))
                return;

            FuelPricesDbContext DbCtx = serviceProvider.GetRequiredService<FuelPricesDbContext>();

            List<EstacionServicio> NewObjects = new(Resultado.Estaciones.Length);

            DateTimeOffset AhoraUtc = DateTimeOffset.UtcNow;

            for (int i = 0; i < Resultado.Estaciones.Length; i++)
            {
                EstacionesServicioJson EstacionServicioJson = Resultado.Estaciones[i];
                int IdEs = int.Parse(EstacionServicioJson.IDEESS);
                int IdMunicipio = int.Parse(EstacionServicioJson.IDMunicipio);

                NewObjects.Add(new EstacionServicio()
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
        }
        catch (Exception e) when (Logger.Handle(e, $"Error on '{OrigenRegistros}' failed.")) { }
    }

    private async Task LoadEstacionProductoPrecioAsync(IServiceProvider serviceProvider, int today, CancellationToken cancellationToken)
    {
        const string OrigenRegistros = "EstacionServicioProducto";

        try
        {
            FuelPricesDbContext DbCtx = serviceProvider.GetRequiredService<FuelPricesDbContext>();

            int[] ProductoPetroliferoIds = await DbCtx.ProductosPetroliferos
                .Select(x => x.IdProducto)
                .OrderBy(x => x)
                .ToArrayAsync(cancellationToken);

            DateTimeOffset AhoraUtc = DateTimeOffset.UtcNow;

            for (int i = 0; i < ProductoPetroliferoIds.Length; i++)
            {
                int ProductoPetroliferoId = ProductoPetroliferoIds[i];

                EstacionServicioProductoRootJson? Resultado = await LoadJsonAsync<EstacionServicioProductoRootJson>(
                    string.Concat(Settings.Minetur.Uris.EstacionesTerrestres, string.Format(Settings.Minetur.Uris.EstacionesTerrestresFiltroProducto, ProductoPetroliferoId)),
                    cancellationToken);

                if (!(Resultado?.EstacionServicioProducto.Length > 0))
                    continue;

                List<EstacionProductoPrecio> NewObjects = new(Resultado.EstacionServicioProducto.Length);

                for (int j = 0; j < Resultado.EstacionServicioProducto.Length; j++)
                {
                    EstacionServicioProductoJson EstacionServicioProducto = Resultado.EstacionServicioProducto[j];
                    int EstacionServicioId = int.Parse(EstacionServicioProducto.IDEESS);
                    int NuevoPrecio = (int)(decimal.Parse(EstacionServicioProducto.PrecioProducto, Libs.Utils.Constants.Formats.ESCultureInfo) * 1_000);

                    NewObjects.Add(new EstacionProductoPrecio()
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
}
