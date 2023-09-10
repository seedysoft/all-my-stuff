using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Seedysoft.UtilsLib.Extensions;
using Serilog;

namespace Seedysoft.Carburantes.ConsoleApp;

public class Program
{
    private const string ApplicationName = $"{nameof(Carburantes)}{nameof(ConsoleApp)}";

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
                    .AddJsonFile("appsettings.Infrastructure.json", false, true)
                    .AddJsonFile($"appsettings.Infrastructure.{CurrentEnvironmentName}.json", false, true)

                    .AddJsonFile($"appsettings.CarburantesConnectionStrings.{CurrentEnvironmentName}.json", false, true)

                    .AddJsonFile($"appsettings.Serilog.{CurrentEnvironmentName}.json", false, true);
            })
            .ConfigureLogging((hostBuilderContext, iLoggingBuilder) =>
            {
                IConfigurationSection configurationSection = hostBuilderContext.Configuration.GetRequiredSection("Serilog:WriteTo:1:Args:path")!;
                configurationSection.Value = configurationSection.Value!.Replace("{ApplicationName}", ApplicationName);

                _ = iLoggingBuilder.AddSerilog(new LoggerConfiguration()
                    .ReadFrom.Configuration(hostBuilderContext.Configuration)
                    .CreateLogger());
            })
            .ConfigureServices((hostBuilderContext, serviceCollection) =>
            {
                Infrastructure.Dependencies.ConfigureServices(hostBuilderContext.Configuration, serviceCollection);

                _ = serviceCollection.AddHttpClient(nameof(Core.Settings.Minetur));
            }).Build();

        ILogger<Program> Logger = host.Services.GetRequiredService<ILogger<Program>>();

        SQLitePCL.Batteries.Init();

        Logger.LogInformation($"Called {ApplicationName}.");

        try
        {
            Logger.Information($"{ApplicationName} starts");

            using CancellationTokenSource CancelTokenSource = new();

            // Migrate and seed the database during startup. Must be synchronous.
            try
            {
                using IServiceScope Scope = host.Services.CreateScope();

                Scope.ServiceProvider.GetRequiredService<Infrastructure.Data.CarburantesDbContext>().Database.Migrate();
                Scope.ServiceProvider.GetRequiredService<Infrastructure.Data.CarburantesHistDbContext>().Database.Migrate();
            }
            catch (Exception e) when (Logger.Handle(e, "Unhandled exception.")) { }

            Services.ObtainDataCronBackgroundService ObtainDataSvc = new(host.Services);

            await ObtainDataSvc.MainAsync(CancelTokenSource.Token);
        }
        catch (TaskCanceledException e) when (Logger.Handle(e, "Task cancelled.")) { }
        catch (Exception e) when (Logger.Handle(e, "Unhandled exception.")) { }
        finally { await Task.CompletedTask; }

        Logger.LogInformation($"End {ApplicationName}.");

        Environment.Exit(0);
    }
}
