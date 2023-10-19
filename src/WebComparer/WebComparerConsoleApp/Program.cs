using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Seedysoft.WebComparerConsoleApp;

public class Program
{
    public static async Task Main(string[] args)
    {
        IHostBuilder builder = new HostBuilder();

        InfrastructureLib.Dependencies.ConfigureDefaultDependencies(builder, args);

        _ = builder

            .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
            {
                string CurrentEnvironmentName = hostBuilderContext.HostingEnvironment.EnvironmentName;

                _ = configurationBuilder
                   .AddJsonFile($"appsettings.dbConnectionString.{CurrentEnvironmentName}.json", false, true)
                   .AddJsonFile($"appsettings.WebComparerSettings.{CurrentEnvironmentName}.json", false, true);
            })

            .ConfigureServices((hostBuilderContext, iServiceCollection) =>
            {
                InfrastructureLib.Dependencies.AddDbCxtContext(hostBuilderContext.Configuration, iServiceCollection);

                iServiceCollection.TryAddSingleton(hostBuilderContext.Configuration.GetSection(nameof(WebComparerLib.Settings.WebComparerSettings)).Get<WebComparerLib.Settings.WebComparerSettings>()!);
                iServiceCollection.TryAddSingleton<WebComparerLib.Services.WebComparerCronBackgroundService>();
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
            {
                Scope.ServiceProvider.GetRequiredService<InfrastructureLib.DbContexts.DbCxt>().Database.Migrate();

                using CancellationTokenSource CancelTokenSource = new();
                {
                    using WebComparerLib.Services.WebComparerCronBackgroundService webComparerCronBackgroundService = host.Services.GetRequiredService<WebComparerLib.Services.WebComparerCronBackgroundService>();

                    await webComparerCronBackgroundService.DoWorkAsync(CancelTokenSource.Token);
                }
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
