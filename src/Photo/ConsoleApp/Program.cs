using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Core.Extensions;
using Seedysoft.Libs.Infrastructure.Extensions;

namespace Seedysoft.Photo.ConsoleApp;

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
                serviceScope.ServiceProvider.GetRequiredService<Lib.DbContexts.PhotoCtx>().Database.Migrate();
            }

            using CancellationTokenSource CancelTokenSource = new();

            using (Lib.Services.PhotoCronBackgroundService photoCronBackgroundService = host.Services.GetRequiredService<Lib.Services.PhotoCronBackgroundService>())
                await photoCronBackgroundService.DoWorkAsync(CancelTokenSource.Token);

            if (Logger.IsEnabled(LogLevel.Information))
                Logger.LogInformation("End {ApplicationName}", AppName);
        }
        catch (TaskCanceledException) { /* ignored */  }
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
