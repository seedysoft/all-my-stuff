using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Seedysoft.ObtainPvpcConsoleApp;

public class Program
{
    public static async Task Main(string[] args)
    {
        IHostBuilder builder = new HostBuilder();

        InfrastructureLib.Dependencies.ConfigureDefaultDependencies(builder, args);

        builder
            .ConfigureAppConfiguration((hostBuilderContext, iConfigurationBuilder) =>
            {
                string CurrentEnvironmentName = hostBuilderContext.HostingEnvironment.EnvironmentName;

                _ = iConfigurationBuilder
                    .AddJsonFile($"appsettings.dbConnectionString.{CurrentEnvironmentName}.json", false, true);
            })

            .ConfigureServices((hostBuilderContext, iServiceCollection) =>
                InfrastructureLib.Dependencies.AddDbContext<DbContexts.DbCxt>(hostBuilderContext.Configuration, iServiceCollection));

        ObtainPvpcLib.Services.ObtainPvpCronBackgroundService.Configure(builder);

        IHost host = builder.Build();

        ILogger<Program> Logger = host.Services.GetRequiredService<ILogger<Program>>();

        Logger.LogInformation("Called {ApplicationName} version {Version}", host.Services.GetRequiredService<IHostEnvironment>().ApplicationName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        try
        {
            //System.Collections.IDictionary? EnvVariables = Environment.GetEnvironmentVariables();
            //foreach (object? item in EnvVariables.Keys)
            //    Logger.LogDebug($"{item}: {EnvVariables[item]}");
            //IEnumerable<KeyValuePair<string, string?>> Config = host.Services.GetRequiredService<IConfiguration>().AsEnumerable();
            //foreach (KeyValuePair<string, string?> item in Config)
            //    Logger.LogDebug($"{item.Key}: {item.Value ?? "<<NULL>>"}");

#if DEBUG
            await Task.Delay(UtilsLib.Constants.Time.TenSecondsTimeSpan);
#endif

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
            Logger.LogInformation("Requesting PVPCs for {ForDate}", ForDate.ToString(UtilsLib.Constants.Formats.YearMonthDayFormat));

            DbContexts.DbCxt dbCtx = host.Services.GetRequiredService<DbContexts.DbCxt>();

            int? HowManyPricesObtained = await ObtainPvpcLib.Main.ObtainPricesAsync(
                dbCtx,
                host.Services.GetRequiredService<ObtainPvpcLib.Settings.ObtainPvpcSettings>(),
                Logger,
                ForDate,
                CancellationToken.None);

            Logger.LogInformation("End {ApplicationName}", host.Services.GetRequiredService<IHostEnvironment>().ApplicationName);
        }
        catch (TaskCanceledException) { /* ignored */ }
        catch (Exception e) { Logger.LogError(e, "Unexpected Error"); }
        finally { await Task.CompletedTask; }

        Environment.Exit(0);
    }
}
