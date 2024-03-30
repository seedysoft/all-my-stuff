using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Seedysoft.InfrastructureLib.Extensions;
using Seedysoft.UtilsLib.Extensions;

namespace Seedysoft.Carburantes.ConsoleApp;

public sealed class Program
{
    public static async Task Main(string[] args)
    {
        HostApplicationBuilder builder = new();

        InfrastructureLib.Dependencies.ConfigureDefaultDependencies(builder, args);

        _ = builder.Configuration
            .AddJsonFile($"appsettings.Carburantes.Infrastructure.json", false, true)
            .AddJsonFile($"appsettings.Carburantes.Infrastructure.{builder.Environment.EnvironmentName}.json", false, true)
            .AddJsonFile($"appsettings.CarburantesConnectionStrings.{builder.Environment.EnvironmentName}.json", false, true);

        _ = builder.Services.AddDbContext<Infrastructure.Data.CarburantesDbContext>((iServiceProvider, dbContextOptionsBuilder) =>
        {
            string ConnectionStringName = nameof(Infrastructure.Data.CarburantesDbContext);
            string ConnectionString = builder.Configuration.GetConnectionString($"{ConnectionStringName}") ?? throw new KeyNotFoundException($"Connection string '{ConnectionStringName}' not found.");
            string FullFilePath = Path.GetFullPath(ConnectionString[CoreLib.Constants.DatabaseStrings.DataSource.Length..]);
            if (!File.Exists(FullFilePath))
                throw new FileNotFoundException("Database file not found.", FullFilePath);

            _ = dbContextOptionsBuilder.UseSqlite($"{CoreLib.Constants.DatabaseStrings.DataSource}{FullFilePath}");
            dbContextOptionsBuilder.ConfigureDebugOptions();
        }
        , ServiceLifetime.Transient
        , ServiceLifetime.Transient)

        .AddDbContext<Infrastructure.Data.CarburantesHistDbContext>((iServiceProvider, dbContextOptionsBuilder) =>
        {
            string ConnectionStringName = nameof(Infrastructure.Data.CarburantesHistDbContext);
            string ConnectionString = builder.Configuration.GetConnectionString($"{ConnectionStringName}") ?? throw new KeyNotFoundException($"Connection string '{ConnectionStringName}' not found.");
            string FullFilePath = Path.GetFullPath(ConnectionString[CoreLib.Constants.DatabaseStrings.DataSource.Length..]);
            if (!File.Exists(FullFilePath))
                throw new FileNotFoundException("Database file not found.", FullFilePath);

            _ = dbContextOptionsBuilder.UseSqlite($"{CoreLib.Constants.DatabaseStrings.DataSource}{FullFilePath}");
            dbContextOptionsBuilder.ConfigureDebugOptions();
        }
        , ServiceLifetime.Transient
        , ServiceLifetime.Transient);

        _ = builder.Services.AddHttpClient(nameof(Core.Settings.Minetur));

        builder.Services.TryAddSingleton<Services.ObtainDataCronBackgroundService>();

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

            Scope.ServiceProvider.GetRequiredService<Infrastructure.Data.CarburantesDbContext>().Database.Migrate();
            Scope.ServiceProvider.GetRequiredService<Infrastructure.Data.CarburantesHistDbContext>().Database.Migrate();

            Services.ObtainDataCronBackgroundService obtainDataCronBackgroundService = host.Services.GetRequiredService<Services.ObtainDataCronBackgroundService>();

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
