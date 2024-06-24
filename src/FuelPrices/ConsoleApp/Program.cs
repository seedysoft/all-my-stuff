using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Utils.Extensions;

namespace Seedysoft.FuelPrices.ConsoleApp;

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
            using CancellationTokenSource CancelTokenSource = new();

            // Migrate and seed the database during startup. Must be synchronous.
            using IServiceScope Scope = host.Services.CreateScope();

            Scope.ServiceProvider.GetRequiredService<Lib.Infrastructure.Data.FuelPricesDbContext>().Database.Migrate();

            Lib.Services.ObtainFuelPricesService obtainFuelPricesService = host.Services.GetRequiredService<Lib.Services.ObtainFuelPricesService>();

            await obtainFuelPricesService.DoWorkAsync(CancelTokenSource.Token);
        }
        catch (TaskCanceledException e) when (Logger.Handle(e, "Task cancelled.")) { }
        catch (Exception e) when (Logger.Handle(e, "Unhandled exception.")) { }
        finally { await Task.CompletedTask; }

        Logger.LogInformation("End {ApplicationName}", AppName);

        if (System.Diagnostics.Debugger.IsAttached)
        {
            Console.WriteLine("Press Intro to exit");
            _ = Console.ReadLine();
        }

        Environment.Exit(0);
    }
}
