using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Core.Extensions;
using Seedysoft.Libs.Infrastructure.Extensions;

namespace Seedysoft.Pvpc.ConsoleApp;

public sealed class Program : Libs.Infrastructure.ProgramBase
{
    [STAThread]
    public static async Task Main(string[] args)
    {
        Microsoft.Extensions.Hosting.HostApplicationBuilder hostBuilder = CreateHostBuilder(args);

        _ = hostBuilder.AddAllMyDependencies();

        Microsoft.Extensions.Hosting.IHost host = hostBuilder.Build();

        ILogger<Program> Logger = host.Services.GetRequiredService<ILogger<Program>>();

        string AppName = host.Services.GetRequiredService<Microsoft.Extensions.Hosting.IHostEnvironment>().ApplicationName;

        if (Logger.IsEnabled(LogLevel.Information))
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
            using IServiceScope serviceScope = host.Services.CreateScope();
            {
                serviceScope.ServiceProvider.GetRequiredService<Libs.Infrastructure.DbContexts.DbCtx>().Database.Migrate();
            }
            // var host = CreateHostBuilder(args).Build();
            //   using (var scope = host.Services.CreateScope()) {
            //     var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //     db.Database.Migrate();...

            DateTime ForDate = DateTime.MinValue;

            if (args?.Length < 1
                || !DateTime.TryParseExact(
                    args?[0],
                    Libs.Core.Constants.Formats.YearMonthDay,
                    Libs.Core.Constants.Globalization.CultureInfoInvariant,
                    System.Globalization.DateTimeStyles.None,
                    out ForDate))
            {
                if (Logger.IsEnabled(LogLevel.Information))
                    Logger.LogInformation($"You can provide the date in {Libs.Core.Constants.Formats.YearMonthDay} format as an argument");
                ForDate = DateTimeOffset.UtcNow.AddDays(1).Date;
            }

            using CancellationTokenSource CancelTokenSource = new();

            using (Lib.Services.PvpcCronBackgroundService pvpcCronBackgroundService = host.Services.GetRequiredService<Lib.Services.PvpcCronBackgroundService>())
                await pvpcCronBackgroundService.GetPvpcFromReeForDateAsync(ForDate, CancelTokenSource.Token);

            if (Logger.IsEnabled(LogLevel.Information))
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
