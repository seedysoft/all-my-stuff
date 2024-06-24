using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Seedysoft.Libs.Infrastructure.Extensions;

namespace Seedysoft.FuelPrices.Lib.Dependencies;

internal sealed class Configurator : Libs.Utils.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder)
    {
        string CurrentEnvironmentName = hostApplicationBuilder.Environment.EnvironmentName;

        _ = hostApplicationBuilder.Configuration
            .AddJsonFile($"appsettings.FuelPrices.Lib.ConnectionStrings.{CurrentEnvironmentName}.json", false, true)
            .AddJsonFile($"appsettings.FuelPrices.Lib.json", false, true)
            .AddJsonFile($"appsettings.FuelPrices.Lib.{CurrentEnvironmentName}.json", false, true);
    }

    protected override void AddDbContexts(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder)
    {
        _ = hostApplicationBuilder.Services.AddDbContext<Infrastructure.Data.FuelPricesDbContext>((iServiceProvider, dbContextOptionsBuilder) =>
        {
            string ConnectionStringName = nameof(Lib.Infrastructure.Data.FuelPricesDbContext);
            string ConnectionString = hostApplicationBuilder.Configuration.GetConnectionString($"{ConnectionStringName}") ?? throw new KeyNotFoundException($"Connection string '{ConnectionStringName}' not found.");
            //string FullFilePath = Path.GetFullPath(
            //    ConnectionString[Libs.Core.Constants.DatabaseStrings.DataSource.Length..],
            //    System.Reflection.Assembly.GetExecutingAssembly().Location);
            //if (!File.Exists(FullFilePath))
            //    throw new FileNotFoundException("Database file not found.", FullFilePath);

            //_ = dbContextOptionsBuilder.UseSqlite($"{Libs.Core.Constants.DatabaseStrings.DataSource}{FullFilePath}");
            _ = dbContextOptionsBuilder.UseSqlite(ConnectionString);
            dbContextOptionsBuilder.ConfigureDebugOptions();
        }
        , ServiceLifetime.Singleton
        , ServiceLifetime.Singleton);
    }

    protected override void AddMyServices(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder)
    {
        _ = hostApplicationBuilder.Services.AddHttpClient(nameof(FuelPrices.Lib.Core.Settings.Minetur));
        hostApplicationBuilder.Services.TryAddSingleton<Services.ObtainDataCronBackgroundService>();
        _ = hostApplicationBuilder.Services.AddHostedService<Services.ObtainDataCronBackgroundService>();
    }
}
