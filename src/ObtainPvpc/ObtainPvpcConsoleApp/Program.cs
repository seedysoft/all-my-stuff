using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Seedysoft.ObtainPvpcConsoleApp;

public class Program
{
    private const string ApplicationName = nameof(ObtainPvpcConsoleApp);

    public static async Task Main(string[] args)
    {
        IHost host = new HostBuilder()
            .ConfigureHostConfiguration(iConfigurationBuilder =>
            {
                _ = iConfigurationBuilder
                    .AddCommandLine(args)
                    .AddEnvironmentVariables();
            })
            .ConfigureAppConfiguration((hostBuilderContext, iConfigurationBuilder) =>
            {
                string CurrentEnvironmentName = hostBuilderContext.HostingEnvironment.EnvironmentName;

                _ = iConfigurationBuilder
                    .AddJsonFile($"appsettings.ObtainPvpcSettings.json", false, true)

                    .AddJsonFile($"appsettings.Serilog.{CurrentEnvironmentName}.json", false, true)

                    .AddJsonFile($"appsettings.dbConnectionString.{CurrentEnvironmentName}.json", false, true);
            })
            .ConfigureLogging((hostBuilderContext, iLoggingBuilder) =>
            {
                IConfigurationSection configurationSection = hostBuilderContext.Configuration.GetRequiredSection("Serilog:WriteTo:1:Args:path");
                configurationSection.Value = Path.GetFullPath(configurationSection.Value!.Replace("{ApplicationName}", ApplicationName));

                _ = iLoggingBuilder.AddSerilog(new LoggerConfiguration()
                    .ReadFrom.Configuration(hostBuilderContext.Configuration)
                    .CreateLogger());
            })
            .ConfigureServices((hostBuilderContext, iServiceCollection) =>
            {
                iServiceCollection.TryAddSingleton(hostBuilderContext.Configuration.GetSection(nameof(ObtainPvpcLib.Settings.ObtainPvpcSettings)).Get<ObtainPvpcLib.Settings.ObtainPvpcSettings>()!);

                _ = iServiceCollection.AddDbContext<DbContexts.DbCxt>(dbContextOptionsBuilder =>
                {
                    const string ConnectionStringName = nameof(DbContexts.DbCxt);
                    string ConnectionString = hostBuilderContext.Configuration.GetConnectionString($"{ConnectionStringName}") ?? throw new KeyNotFoundException($"Connection string '{ConnectionStringName}' not found.");
                    string FullFilePath = Path.GetFullPath(ConnectionString["Data Source=".Length..]);
                    if (!File.Exists(FullFilePath))
                        throw new FileNotFoundException("Database file not found: '{FullPath}'", FullFilePath);

                    dbContextOptionsBuilder.UseSqlite(ConnectionString);

                    dbContextOptionsBuilder.EnableDetailedErrors();
                    dbContextOptionsBuilder.EnableSensitiveDataLogging();
#if DEBUG
                    dbContextOptionsBuilder.LogTo(Console.WriteLine, LogLevel.Trace);
#endif
                });
            })
            .Build();

        SQLitePCL.Batteries.Init();

        ILogger<Program> Logger = host.Services.GetRequiredService<ILogger<Program>>();

        Logger.LogInformation("Called {ApplicationName} version {Version}", ApplicationName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

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

            Logger.LogInformation($"End {ApplicationName}");
        }
        catch (TaskCanceledException) { /* ignored */ }
        catch (Exception e) { Logger.LogError(e, "Unexpected Error"); }
        finally { await Task.CompletedTask; }

        Environment.Exit(0);
    }
}
