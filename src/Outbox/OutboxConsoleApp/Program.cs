﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Seedysoft.UtilsLib.Extensions;

namespace Seedysoft.OutboxConsoleApp;

public sealed class Program
{
    public static async Task Main(string[] args)
    {
        HostApplicationBuilder hostApplicationBuilder = new(args);

        _ = hostApplicationBuilder.AddAllMyDependencies();

        IHost host = hostApplicationBuilder.Build();

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

            using CancellationTokenSource CancelTokenSource = new();

            using (OutboxLib.Services.OutboxCronBackgroundService outboxCronBackgroundService = host.Services.GetRequiredService<OutboxLib.Services.OutboxCronBackgroundService>())
                await outboxCronBackgroundService.DoWorkAsync(CancelTokenSource.Token);

            Logger.LogInformation("End {ApplicationName}", AppName);
        }
        catch (TaskCanceledException) { /* ignored */  }
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
