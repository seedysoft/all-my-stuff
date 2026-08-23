using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Core.Extensions;
using Seedysoft.Libs.Infrastructure.Extensions;

namespace Seedysoft.Libs.GasStationPrices.ConsoleApp;

public sealed class Program : Core.ProgramBase
{
    [STAThread]
    public static async Task Main(string[] args)
    {
        await ObtainCommandLineAsync(args);

        Microsoft.Extensions.Hosting.HostApplicationBuilder hostApplicationBuilder = new(args);

        _ = hostApplicationBuilder.AddAllMyDependencies();

        Microsoft.Extensions.Hosting.IHost host = hostApplicationBuilder.Build();

        ILogger<Program> Logger = host.Services.GetRequiredService<ILogger<Program>>();

        string AppName = host.Services.GetRequiredService<Microsoft.Extensions.Hosting.IHostEnvironment>().ApplicationName;

        if (Logger.IsEnabled(LogLevel.Information))
            Logger.LogInformation("Called {ApplicationName} version {Version}", AppName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        try
        {
            //System.Collections.IDictionary? EnvVariables = Environment.GetEnvironmentVariables();
            //foreach (object? item in EnvVariables.Keys)
            //    Logger.LogDebug($"{item}: {EnvVariables[item]}");
            //IEnumerable<KeyValuePair<string, string?>> Config = host.Services.GetRequiredService<IConfiguration>().AsEnumerable();
            //foreach (KeyValuePair<string, string?> item in Config)
            //    Logger.LogDebug($"{item.Key}: {item.Value ?? "<<NULL>>"}");

            //// Migrate and seed the database during startup. Must be synchronous.
            //using AsyncServiceScope Scope = host.Services.CreateAsyncScope();
            //await Scope.ServiceProvider.GetRequiredService<Infrastructure.DbContexts.DbCxt>().Database.MigrateAsync();

            Console.WriteLine("Awaiting debugger connection...");

            while (!System.Diagnostics.Debugger.IsAttached)
                await Task.Delay(1_000);

            Console.WriteLine("...debugger connected!");

            using CancellationTokenSource CancelTokenSource = new();
            {
                Services.GasStationPricesService gasStationPricesService = host.Services.GetRequiredService<Services.GasStationPricesService>();
                Travel.Models.Bounds bounds = new(
                    new Travel.Models.Location(Travel.Constants.Earth.Burgos.Latitude, Travel.Constants.Earth.Burgos.Longitude),
                    new Travel.Models.Location(Travel.Constants.Earth.Brazuelo.Latitude, Travel.Constants.Earth.Brazuelo.Longitude));

                //IReadOnlyList<ViewModels.GasStationModel> Result =
                //    await gasStationPricesService.GetNearGasStationsAsync(
                //        bounds: bounds,
                //        maxDistanceInKm: 10,
                //        cancellationToken: CancelTokenSource.Token);

                await Task.Delay(TimeSpan.FromMinutes(1));
            }

            if (Logger.IsEnabled(LogLevel.Information))
                Logger.LogInformation("End {ApplicationName}", AppName);
        }
        catch (TaskCanceledException) { /* ignored */ }
        catch (Exception e) { _ = Logger.LogAndHandle(e, "Unexpected Error"); }
        finally { await Task.CompletedTask; }

        if (System.Diagnostics.Debugger.IsAttached)
        {
            Console.WriteLine("Press Intro to exit");
            _ = Console.ReadLine();
        }

        Environment.Exit(0);
    }
}
