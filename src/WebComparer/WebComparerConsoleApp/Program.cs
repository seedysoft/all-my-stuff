using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Seedysoft.WebComparerConsoleApp;

public class Program
{
    public static async Task Main(string[] args)
    {
        IHostBuilder builder = new  HostBuilder();

        InfrastructureLib.Dependencies.ConfigureDefaultDependencies(builder, args);

        builder
            .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
                _ = configurationBuilder.AddJsonFile($"appsettings.dbConnectionString.{hostBuilderContext.HostingEnvironment.EnvironmentName}.json", false, true))

            .ConfigureServices((hostBuilderContext, iServiceCollection) =>
                InfrastructureLib.Dependencies.AddDbContext<DbContexts.DbCxt>(hostBuilderContext.Configuration, iServiceCollection));

        IHost host =builder.Build();

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

            using DbContexts.DbCxt dbCtx = host.Services.GetRequiredService<DbContexts.DbCxt>();

            await WebComparerLib.Main.FindDifferencesAsync(dbCtx, Logger);

            Logger.LogInformation("End {ApplicationName}", host.Services.GetRequiredService<IHostEnvironment>().ApplicationName);
        }
        catch (TaskCanceledException) { /* ignored */ }
        catch (Exception e) { Logger.LogError(e, "Unexpected Error"); }
        finally { await Task.CompletedTask; }

        Environment.Exit(0);
    }
}
