using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Seedysoft.CarburantesLib.Infrastructure.Data;
using Seedysoft.CarburantesLib.Services;

namespace Seedysoft.CarburantesLib.Dependencies;

internal sealed class Configurator : UtilsLib.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(IHostApplicationBuilder hostApplicationBuilder)
    {
        string CurrentEnvironmentName = hostApplicationBuilder.Environment.EnvironmentName;

        _ = hostApplicationBuilder.Configuration
            .AddJsonFile($"appsettings.CarburantesConnectionStrings.{CurrentEnvironmentName}.json", false, true)
            .AddJsonFile($"appsettings.Carburantes.Infrastructure.json", false, true)
            .AddJsonFile($"appsettings.Carburantes.Infrastructure.{CurrentEnvironmentName}.json", false, true);
    }

    protected override void AddDbContexts(IHostApplicationBuilder hostApplicationBuilder)
    {
        //_ = hostApplicationBuilder.Services
        //    .AddDbContextFactory<CarburantesDbContext>((IServiceProvider serviceProvider, DbContextOptionsBuilder dbContextOptionsBuilder) =>
        //    {
        //        dbContextOptionsBuilder.
        //        //DateOnly FirstObtained = new(2023, 7, 12);
        //        //DateOnly Today = DateOnly.MinValue;

        //        //string YearMonth = fullDay.ToString()[..4];

        //        //CarburantesDbContext? carburantesDbContext = ServiceProvider.GetKeyedService<CarburantesDbContext>(YearMonth);
        //        //if (carburantesDbContext != null)
        //        //    return carburantesDbContext;

        //        //string ConnectionString = ServiceProvider.GetRequiredService<IConfiguration>().GetConnectionString($"{ConnectionStringName}") ?? throw new KeyNotFoundException($"Connection string '{ConnectionStringName}' not found.");

        //        //string FullFilePath = Path.GetFullPath(
        //        //    ConnectionString.Replace("{yyMM}", YearMonth),
        //        //    System.Reflection.Assembly.GetExecutingAssembly().Location);

        //        //DbContextOptions<CarburantesDbContext> dbContextOptions = new();
        //        //DbContextOptionsBuilder dbContextOptionsBuilder = new(dbContextOptions);
        //        //_ = dbContextOptionsBuilder.UseSqlite($"{UtilsLib.Constants.DatabaseStrings.DataSource}{FullFilePath}");
        //        //dbContextOptionsBuilder.ConfigureDebugOptions();

        //        //carburantesDbContext = new(dbContextOptions);

        //        //carburantesDbContext.Database.Migrate();

        //        //return carburantesDbContext;
        //    }
        //, ServiceLifetime.Singleton
        //);
    }

    protected override void AddMyServices(IHostApplicationBuilder hostApplicationBuilder)
    {
        _ = hostApplicationBuilder.Services.AddHttpClient(nameof(Carburantes.CoreLib.Settings.Minetur));
        hostApplicationBuilder.Services.TryAddSingleton<ObtainDataCronBackgroundService>();
        _ = hostApplicationBuilder.Services.AddHostedService<ObtainDataCronBackgroundService>();
    }
}
