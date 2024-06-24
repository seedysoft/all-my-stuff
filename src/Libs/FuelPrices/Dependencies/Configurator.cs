using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Seedysoft.Libs.FuelPrices.Infrastructure.Data;
using Seedysoft.Libs.Infrastructure.Extensions;

namespace Seedysoft.Libs.FuelPrices.Dependencies;

internal sealed class Configurator : Utils.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder)
    {
        string CurrentEnvironmentName = hostApplicationBuilder.Environment.EnvironmentName;

        _ = hostApplicationBuilder.Configuration
            .AddJsonFile($"appsettings.Libs.FuelPrices.ConnectionStrings.{CurrentEnvironmentName}.json", false, true)
            .AddJsonFile($"appsettings.Libs.FuelPrices.json", false, true)
            .AddJsonFile($"appsettings.Libs.FuelPrices.{CurrentEnvironmentName}.json", false, true);
    }

    protected override void AddDbContexts(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder)
    {
        _ = hostApplicationBuilder.Services.AddDbContext<FuelPricesDbContext>((iServiceProvider, dbContextOptionsBuilder) =>
        {
            string ConnectionStringName = nameof(FuelPricesDbContext);
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
        _ = hostApplicationBuilder.Services.AddHttpClient(nameof(Core.Settings.Minetur));
        hostApplicationBuilder.Services.TryAddSingleton<Services.ObtainFuelPricesService>();
    }
}
