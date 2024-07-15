using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Utils.Extensions;

namespace Seedysoft.Update.ConsoleApp;

public sealed class Program : Libs.Core.ProgramBase
{
    [STAThread]
    public static async Task Main(string[] args)
    {
        Lib.Settings.UpdateConsoleOptions updateConsoleOptions = await ObtainCommandLineAsync<Lib.Settings.UpdateConsoleOptions>(args);

        Microsoft.Extensions.Hosting.HostApplicationBuilder hostApplicationBuilder = new(args);

        _ = hostApplicationBuilder.AddAllMyDependencies();

        Microsoft.Extensions.Hosting.IHost host = hostApplicationBuilder.Build();

        ILogger<Program> Logger = host.Services.GetRequiredService<ILogger<Program>>();

        string AppName = host.Services.GetRequiredService<Microsoft.Extensions.Hosting.IHostEnvironment>().ApplicationName;

        Logger.LogInformation("Called {ApplicationName} version {Version}", AppName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        try
        {
            // TODO: move to base classes

            //System.Collections.IDictionary? EnvVariables = Environment.GetEnvironmentVariables();
            //foreach (object? item in EnvVariables.Keys)
            //    Logger.LogDebug($"{item}: {EnvVariables[item]}");
            //IEnumerable<KeyValuePair<string, string?>> Config = host.Services.GetRequiredService<IConfiguration>().AsEnumerable();
            //foreach (KeyValuePair<string, string?> item in Config)
            //    Logger.LogDebug($"{item.Key}: {item.Value ?? "<<NULL>>"}");

            // Migrate and seed the database during startup. Must be synchronous.
            using IServiceScope Scope = host.Services.CreateAsyncScope();

            using CancellationTokenSource CancelTokenSource = new();

            using (Lib.Services.UpdateBackgroundServiceCron updateBackgroundServiceCron = host.Services.GetRequiredService<Lib.Services.UpdateBackgroundServiceCron>())
                await updateBackgroundServiceCron.CopyUpdatesAsync(updateConsoleOptions.InstallDirectory, CancelTokenSource.Token);

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
