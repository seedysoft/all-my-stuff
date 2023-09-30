using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Seedysoft.UtilsLib.Extensions;

namespace Seedysoft.Carburantes.ConsoleApp;

public class Program
{
    private static string ApplicationName = string.Empty;

    public static async Task Main(string[] args)
    {
        IHostBuilder builder = new  HostBuilder();
        _ = builder.ConfigureServices((hostBuilderContext, iServiceCollection) =>   ApplicationName= hostBuilderContext.HostingEnvironment.ApplicationName);

        InfrastructureLib.Dependencies.ConfigureDefaultDependencies(builder, args);

        _ = builder
            .ConfigureAppConfiguration((hostBuilderContext, iConfigurationBuilder) =>
            {
                string CurrentEnvironmentName = hostBuilderContext.HostingEnvironment.EnvironmentName;

                _ = iConfigurationBuilder
                    .AddJsonFile("appsettings.Carburantes.Infrastructure.json", false, true)
                    .AddJsonFile($"appsettings.Carburantes.Infrastructure.{CurrentEnvironmentName}.json", false, true)

                    .AddJsonFile($"appsettings.CarburantesConnectionStrings.{CurrentEnvironmentName}.json", false, true);
            })

            .ConfigureServices((hostBuilderContext, serviceCollection) =>
            {
                InfrastructureLib.Dependencies.AddDbContext<Infrastructure.Data.CarburantesDbContext>(hostBuilderContext.Configuration, serviceCollection);
                InfrastructureLib.Dependencies.AddDbContext<Infrastructure.Data.CarburantesHistDbContext>(hostBuilderContext.Configuration, serviceCollection);

                _ = serviceCollection.AddHttpClient(nameof(Core.Settings.Minetur));
            });

        IHost host =builder.Build();

        ILogger<Program> Logger = host.Services.GetRequiredService<ILogger<Program>>();

        Logger.LogInformation("Called {ApplicationName}", ApplicationName);

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

        Logger.LogInformation("End {ApplicationName}", ApplicationName);

        Environment.Exit(0);
    }
}
