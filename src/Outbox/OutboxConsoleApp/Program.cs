using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Seedysoft.OutboxConsoleApp;

public class Program
{
    private static string ApplicationName = string.Empty;

    public static async Task Main(string[] args)
    {
        IHostBuilder builder = new  HostBuilder();
        _ = builder.ConfigureServices((hostBuilderContext, iServiceCollection) => ApplicationName = hostBuilderContext.HostingEnvironment.ApplicationName);

        InfrastructureLib.Dependencies.ConfigureDefaultDependencies(builder, args);

        builder
            .ConfigureAppConfiguration((hostBuilderContext, iConfigurationBuilder) =>
            {
                string CurrentEnvironmentName = hostBuilderContext.HostingEnvironment.EnvironmentName;

                _ = iConfigurationBuilder
                    .AddJsonFile($"appsettings.SmtpServiceSettings.json", false, true)

                    .AddJsonFile($"appsettings.TelegramSettings.{CurrentEnvironmentName}.json", false, true)

                    .AddJsonFile($"appsettings.dbConnectionString.{CurrentEnvironmentName}.json", false, true);
            })

            .ConfigureServices((hostBuilderContext, iServiceCollection) =>
            {
                InfrastructureLib.Dependencies.AddDbContext<DbContexts.DbCxt>(hostBuilderContext.Configuration, iServiceCollection);

                iServiceCollection.TryAddSingleton(hostBuilderContext.Configuration.GetSection(nameof(SmtpServiceLib.Settings.SmtpServiceSettings)).Get<SmtpServiceLib.Settings.SmtpServiceSettings>()!);
                iServiceCollection.TryAddSingleton(hostBuilderContext.Configuration.GetSection(nameof(TelegramLib.Settings.TelegramSettings)).Get<TelegramLib.Settings.TelegramSettings>()!);

                iServiceCollection.TryAddScoped<SmtpServiceLib.Services.SmtpService>();

                iServiceCollection.TryAddSingleton<TelegramLib.Services.TelegramService>();
            });

        IHost host =builder.Build();

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

            await TelegramLib.Main.SendMessagesAsync(host.Services, Logger, CancellationToken.None);

            Logger.LogInformation("End {ApplicationName}", ApplicationName);
        }
        catch (TaskCanceledException) { /* ignored */  }
        catch (Exception e) { Logger.LogError(e, "Unexpected Error"); }
        finally { await Task.CompletedTask; }

        Environment.Exit(0);
    }
}
