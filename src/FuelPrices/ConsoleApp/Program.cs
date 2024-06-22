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

        _ = builder.Configuration
            .AddJsonFile($"appsettings.Infrastructure.json", false, true)
            .AddJsonFile($"appsettings.Infrastructure.{builder.Environment.EnvironmentName}.json", false, true)
            .AddJsonFile($"appsettings.CarburantesConnectionStrings.{builder.Environment.EnvironmentName}.json", false, true);

        _ = builder.Services.AddDbContext<Lib.Infrastructure.Data.CarburantesDbContext>((iServiceProvider, dbContextOptionsBuilder) =>
        {
            string ConnectionStringName = nameof(Lib.Infrastructure.Data.CarburantesDbContext);
            string ConnectionString = builder.Configuration.GetConnectionString($"{ConnectionStringName}") ?? throw new KeyNotFoundException($"Connection string '{ConnectionStringName}' not found.");
            string FullFilePath = Path.GetFullPath(
                ConnectionString[Libs.Core.Constants.DatabaseStrings.DataSource.Length..],
                System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (!File.Exists(FullFilePath))
                throw new FileNotFoundException("Database file not found.", FullFilePath);

            _ = dbContextOptionsBuilder.UseSqlite($"{Libs.Core.Constants.DatabaseStrings.DataSource}{FullFilePath}");
            dbContextOptionsBuilder.ConfigureDebugOptions();
        }
        , ServiceLifetime.Transient
        , ServiceLifetime.Transient)

        .AddDbContext<Lib.Infrastructure.Data.CarburantesHistDbContext>((iServiceProvider, dbContextOptionsBuilder) =>
        {
            string ConnectionStringName = nameof(Lib.Infrastructure.Data.CarburantesHistDbContext);
            string ConnectionString = builder.Configuration.GetConnectionString($"{ConnectionStringName}") ?? throw new KeyNotFoundException($"Connection string '{ConnectionStringName}' not found.");
            string FullFilePath = Path.GetFullPath(
                ConnectionString[Libs.Core.Constants.DatabaseStrings.DataSource.Length..],
                System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (!File.Exists(FullFilePath))
                throw new FileNotFoundException("Database file not found.", FullFilePath);

            _ = dbContextOptionsBuilder.UseSqlite($"{Libs.Core.Constants.DatabaseStrings.DataSource}{FullFilePath}");
            dbContextOptionsBuilder.ConfigureDebugOptions();
        }
        , ServiceLifetime.Transient
        , ServiceLifetime.Transient);

        _ = builder.Services.AddHttpClient(nameof(Lib.Core.Settings.Minetur));

        builder.Services.TryAddSingleton<Lib.Services.ObtainDataCronBackgroundService>();

        SQLitePCL.Batteries.Init();

        IHost host = builder.Build();

        ILogger<Program> Logger = host.Services.GetRequiredService<ILogger<Program>>();

        string AppName = host.Services.GetRequiredService<IHostEnvironment>().ApplicationName;

        Logger.LogInformation("Called {ApplicationName} version {Version}", AppName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        try
        {
            using CancellationTokenSource CancelTokenSource = new();

            // Migrate and seed the database during startup. Must be synchronous.
            using IServiceScope Scope = host.Services.CreateScope();

            Scope.ServiceProvider.GetRequiredService<Lib.Infrastructure.Data.CarburantesDbContext>().Database.Migrate();
            Scope.ServiceProvider.GetRequiredService<Lib.Infrastructure.Data.CarburantesHistDbContext>().Database.Migrate();

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
