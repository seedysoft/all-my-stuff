﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Core.Extensions;
using Seedysoft.Libs.Infrastructure.Extensions;

namespace Seedysoft.Pvpc.ConsoleApp;

public sealed class Program : Libs.Core.ProgramBase
{
    [STAThread]
    public static async Task Main(string[] args)
    {
        await ObtainCommandLineAsync(args);

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

            // Migrate and seed the database during startup. Must be synchronous.
            using AsyncServiceScope Scope = host.Services.CreateAsyncScope();
            await Scope.ServiceProvider.GetRequiredService<Libs.Infrastructure.DbContexts.DbCxt>().Database.MigrateAsync();

            DateTime ForDate = DateTime.MinValue;

            if (args?.Length < 1
                || !DateTime.TryParseExact(
                    args?[0],
                    Libs.Core.Constants.Formats.YearMonthDay,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out ForDate))
            {
                Logger.LogInformation($"You can provide the date in {Libs.Core.Constants.Formats.YearMonthDay} format as an argument");
                ForDate = DateTimeOffset.UtcNow.AddDays(1).Date;
            }

            using CancellationTokenSource CancelTokenSource = new();

            using (Lib.Services.PvpcCronBackgroundService pvpcCronBackgroundService = host.Services.GetRequiredService<Lib.Services.PvpcCronBackgroundService>())
                await pvpcCronBackgroundService.GetPvpcFromReeForDateAsync(ForDate, CancelTokenSource.Token);

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
