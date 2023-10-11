using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Seedysoft.WebComparerConsoleApp;

public class Program
{
    public static async Task Main(string[] args)
    {
        IHostBuilder builder = new HostBuilder();

        InfrastructureLib.Dependencies.ConfigureDefaultDependencies(builder, args);

        builder
            .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
                _ = configurationBuilder.AddJsonFile($"appsettings.dbConnectionString.{hostBuilderContext.HostingEnvironment.EnvironmentName}.json", false, true))

            .ConfigureServices((hostBuilderContext, iServiceCollection) =>
                InfrastructureLib.Dependencies.AddDbContext<DbContexts.DbCxt>(hostBuilderContext.Configuration, iServiceCollection));

        WebComparerLib.Services.WebComparerCronBackgroundService.Configure(builder);

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

#if DEBUG
            await Task.Delay(UtilsLib.Constants.Time.TenSecondsTimeSpan);
#endif

            //using DbContexts.DbCxt dbCtx = host.Services.GetRequiredService<DbContexts.DbCxt>();

            //await WebComparerLib.Main.FindDifferencesAsync(dbCtx, Logger);

            using CancellationTokenSource CancelTokenSource = new();

            await host.RunAsync(CancelTokenSource.Token);

            Logger.LogInformation("End {ApplicationName}", AppName);
        }
        catch (TaskCanceledException) { /* ignored */ }
        catch (Exception e) { Logger.LogError(e, "Unexpected Error"); }
        finally { await Task.CompletedTask; }

        Environment.Exit(0);
    }
}
