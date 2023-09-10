﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Seedysoft.UtilsLib.Extensions;
using Serilog;

namespace Seedysoft.OutboxConsoleApp;

public class Program
{
    private const string ApplicationName = nameof(OutboxConsoleApp);

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
                    .AddJsonFile($"appsettings.SmtpServiceSettings.json", false, true)

                    .AddJsonFile($"appsettings.TelegramSettings.{CurrentEnvironmentName}.json", false, true)

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
            .ConfigureServices((hostBuilderContext, serviceCollection) =>
            {
                _ = serviceCollection.AddDbContext<DbContexts.DbCxt>(dbContextOptionsBuilder =>
                {
                    dbContextOptionsBuilder.UseSqlite(hostBuilderContext.Configuration.GetDbCtx(UtilsLib. Enums.ConnectionMode.ReadWrite));
                    dbContextOptionsBuilder.EnableDetailedErrors();
                    dbContextOptionsBuilder.EnableSensitiveDataLogging();
#if DEBUG
                    dbContextOptionsBuilder.LogTo(Console.WriteLine, LogLevel.Trace);
#endif
                });

                serviceCollection.TryAddScoped<SmtpServiceLib.Services.SmtpService>();

                serviceCollection.TryAddSingleton(hostBuilderContext.Configuration.GetSection(nameof(SmtpServiceLib.Settings.SmtpServiceSettings)).Get<SmtpServiceLib.Settings.SmtpServiceSettings>()!);
                serviceCollection.TryAddSingleton(hostBuilderContext.Configuration.GetSection(nameof(TelegramLib.Settings.TelegramSettings)).Get<TelegramLib.Settings.TelegramSettings>()!);

                serviceCollection.TryAddSingleton<TelegramLib.Services.TelegramService>();
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

            await TelegramLib.Main.SendMessagesAsync(host.Services, Logger, CancellationToken.None);

            Logger.LogInformation($"End {ApplicationName}");
        }
        catch (TaskCanceledException) { /* ignored */  }
        catch (Exception e) { Logger.LogError(e, "Unexpected Error"); }
        finally { await Task.CompletedTask; }

        Environment.Exit(0);
    }
}
