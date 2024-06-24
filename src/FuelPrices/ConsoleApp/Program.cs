using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Utils.Extensions;
using Seedysoft.Libs.Infrastructure.Extensions;

namespace Seedysoft.FuelPrices.ConsoleApp;

public sealed class Program
{
    public static async Task Main(string[] args)
    {
        HostApplicationBuilder builder = new();

        Libs.Infrastructure.Dependencies.ConfigureDefaultDependencies(builder, args);

        string CurrentEnvironmentName = builder.Environment.EnvironmentName;

        _ = builder.Configuration
            .AddJsonFile($"appsettings.FuelPrices.Lib.json", false, true)
            .AddJsonFile($"appsettings.FuelPrices.Lib.{CurrentEnvironmentName}.json", false, true)
            .AddJsonFile($"appsettings.FuelPrices.Lib.ConnectionStrings.{CurrentEnvironmentName}.json", false, true);

        _ = builder.Services.AddDbContext<Lib.Infrastructure.Data.FuelPricesDbContext>((iServiceProvider, dbContextOptionsBuilder) =>
        {
            string ConnectionStringName = nameof(Lib.Infrastructure.Data.FuelPricesDbContext);
            string ConnectionString = builder.Configuration.GetConnectionString($"{ConnectionStringName}") ?? throw new KeyNotFoundException($"Connection string '{ConnectionStringName}' not found.");
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

        _ = builder.Services.AddHttpClient(nameof(Lib.Core.Settings.Minetur));

        builder.Services.TryAddSingleton<Lib.Services.ObtainDataCronBackgroundService>();

        IHost host = builder.Build();

        ILogger<Program> Logger = host.Services.GetRequiredService<ILogger<Program>>();

        string AppName = host.Services.GetRequiredService<IHostEnvironment>().ApplicationName;

        Logger.LogInformation("Called {ApplicationName} version {Version}", AppName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        try
        {
            using CancellationTokenSource CancelTokenSource = new();

            // Migrate and seed the database during startup. Must be synchronous.
            using IServiceScope Scope = host.Services.CreateScope();

            Scope.ServiceProvider.GetRequiredService<Lib.Infrastructure.Data.FuelPricesDbContext>().Database.Migrate();

            Lib.Services.ObtainDataCronBackgroundService obtainDataCronBackgroundService = host.Services.GetRequiredService<Lib.Services.ObtainDataCronBackgroundService>();

            await obtainDataCronBackgroundService.DoWorkAsync(CancelTokenSource.Token);
        }
        catch (TaskCanceledException e) when (Logger.Handle(e, "Task cancelled.")) { }
        catch (Exception e) when (Logger.Handle(e, "Unhandled exception.")) { }
        finally { await Task.CompletedTask; }

        Logger.LogInformation("End {ApplicationName}", AppName);

        if (System.Diagnostics.Debugger.IsAttached)
        {
            Console.WriteLine("Press Intro to exit");
            _ = Console.ReadLine();
        }

        Environment.Exit(0);
    }
}
