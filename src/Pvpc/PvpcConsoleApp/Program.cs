using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Seedysoft.PvpcLib.Services;
using Seedysoft.PvpcLib.Settings;

namespace Seedysoft.PvpcConsoleApp;

public sealed class Program
{
    public static async Task Main(string[] args)
    {
        HostBuilder builder = new();

        InfrastructureLib.Dependencies.ConfigureDefaultDependencies(builder, args);

        _ = builder
            .ConfigureAppConfiguration((hostBuilderContext, iConfigurationBuilder) =>
            {
                string CurrentEnvironmentName = hostBuilderContext.HostingEnvironment.EnvironmentName;

                _ = iConfigurationBuilder
                    .AddJsonFile($"appsettings.dbConnectionString.{CurrentEnvironmentName}.json", false, true)
                    .AddJsonFile($"appsettings.PvpcSettings.json", false, true)
                    .AddJsonFile($"appsettings.TuyaManagerSettings.json", false, true);
            })

            .ConfigureServices((hostBuilderContext, iServiceCollection) =>
            {
                InfrastructureLib.Dependencies.AddDbCxtContext(hostBuilderContext.Configuration, iServiceCollection);

                iServiceCollection.TryAddSingleton(hostBuilderContext.Configuration.GetSection(nameof(PvpcSettings)).Get<PvpcSettings>()!);
                iServiceCollection.TryAddSingleton(hostBuilderContext.Configuration.GetSection(nameof(TuyaManagerSettings)).Get<TuyaManagerSettings>()!);
                iServiceCollection.TryAddSingleton<PvpcCronBackgroundService>();
                iServiceCollection.TryAddSingleton<TuyaManagerCronBackgroundService>();
            });

        IHost host = builder.Build();

        ILogger<Program> Logger = host.Services.GetRequiredService<ILogger<Program>>();

        string AppName = host.Services.GetRequiredService<IHostEnvironment>().ApplicationName;

        Logger.LogInformation("Called {ApplicationName} version {Version}", AppName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        try
        {
            //System.Collections.IDictionary? EnvVariables = Environment.GetEnvironmentVariables();
            //foreach (object? item in EnvVariables.Keys)
            //    Logger.LogDebug($"{item}: {EnvVariables[item]}");
            //IEnumerable<KeyValuePair<string, string?>> Config = host.Services.GetRequiredService<IConfiguration>().AsEnumerable();
            //foreach (KeyValuePair<string, string?> item in Config)
            //    Logger.LogDebug($"{item.Key}: {item.Value ?? "<<NULL>>"}");

            if (System.Diagnostics.Debugger.IsAttached)
                await Task.Delay(UtilsLib.Constants.Time.TenSecondsTimeSpan);

            // Migrate and seed the database during startup. Must be synchronous.
            using IServiceScope Scope = host.Services.CreateScope();

            Scope.ServiceProvider.GetRequiredService<InfrastructureLib.DbContexts.DbCxt>().Database.Migrate();

            DateTime ForDate = DateTime.MinValue;

            if (args?.Length < 1
                || !DateTime.TryParseExact(
                    args?[0]
                    , UtilsLib.Constants.Formats.YearMonthDayFormat
                    , System.Globalization.CultureInfo.InvariantCulture
                    , System.Globalization.DateTimeStyles.None
                    , out ForDate))
            {
                Logger.LogInformation($"You can provide the date in {UtilsLib.Constants.Formats.YearMonthDayFormat} format as an argument");
                ForDate = DateTimeOffset.UtcNow.AddDays(1).Date;
            }

            using CancellationTokenSource CancelTokenSource = new();

            using (PvpcCronBackgroundService pvpcCronBackgroundService = host.Services.GetRequiredService<PvpcCronBackgroundService>())
            {
                await pvpcCronBackgroundService.PvpcForDateAsync(ForDate, CancelTokenSource.Token);
            }

            Logger.LogInformation("End {ApplicationName}", AppName);
        }
        catch (TaskCanceledException) { /* ignored */ }
        catch (Exception e) { Logger.LogError(e, "Unexpected Error"); }
        finally { await Task.CompletedTask; }

        if (System.Diagnostics.Debugger.IsAttached)
        {
            Console.WriteLine("Press Intro to exit");
            _ = Console.ReadLine();
        }

        Environment.Exit(0);
    }
}
