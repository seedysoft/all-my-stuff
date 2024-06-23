using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Seedysoft.Libs.Core.Constants;
using Seedysoft.Libs.Infrastructure;
using Seedysoft.Libs.Infrastructure.Extensions;
using Seedysoft.Libs.Telegram.Services;
using Seedysoft.Libs.Telegram.Settings;
using Seedysoft.Outbox.Lib.Services;
using Seedysoft.Pvpc.Lib.Services;

namespace Seedysoft.BlazorWebApp.Server.Extensions;

public static class ProgramStartupExtensions
{
    public static WebApplicationBuilder AddMyDependencies(this WebApplicationBuilder webApplicationBuilder)
    {
        return webApplicationBuilder
            .AddJsonFiles()
            .AddDbContexts()
            .AddMyServices();
    }

    public static WebApplication MigrateDbContexts(this WebApplication webApplication)
    {
        webApplication.Services.GetRequiredService<InfrastructureLib.DbContexts.DbCxt>().Database.Migrate();
        webApplication.Services.GetRequiredService<FuelPrices.Lib.Infrastructure.Data.FuelPricesDbContext>().Database.Migrate();

        return webApplication;
    }

    public static WebApplication SetApiEndpoints(this WebApplication webApplication)
    {
        // Set up API endpoints and methods
        RouteGroupBuilder todoItems = webApplication.MapGroup("/movies");

        //_ = todoItems.MapGet("/", GetAllMovies);
        //_ = todoItems.MapGet("/watched", GetWatchedMovies);
        //_ = todoItems.MapGet("/{id}", GetMovie);
        //_ = todoItems.MapPost("/", CreateMovie);
        //_ = todoItems.MapPut("/{id}", UpdateMovie);
        //_ = todoItems.MapDelete("/{id}", DeleteMovie);

        _ = webApplication.MapControllers();

        return webApplication;
    }

    private static WebApplicationBuilder AddJsonFiles(this WebApplicationBuilder webApplicationBuilder)
    {
        string CurrentEnvironmentName = webApplicationBuilder.Environment.EnvironmentName;

        _ = webApplicationBuilder.Configuration
            .AddJsonFile($"appsettings.BlazorWebApp.Server.json", false, true)
            .AddJsonFile($"appsettings.BlazorWebApp.Server.{CurrentEnvironmentName}.json", false, true)

            .AddJsonFile($"appsettings.FuelPrices.Lib.ConnectionStrings.{CurrentEnvironmentName}.json", false, true)

            .AddJsonFile($"appsettings.FuelPrices.Lib.json", false, true)
            .AddJsonFile($"appsettings.FuelPrices.Lib.{CurrentEnvironmentName}.json", false, true)

            .AddJsonFile($"appsettings.Serilog.json", false, true)
            .AddJsonFile($"appsettings.Serilog.{CurrentEnvironmentName}.json", false, true)

            .AddJsonFile($"appsettings.PvpcSettings.json", false, true)
            .AddJsonFile($"appsettings.SmtpServiceSettings.json", false, true)
            .AddJsonFile($"appsettings.TelegramSettings.json", false, true)
            .AddJsonFile($"appsettings.TelegramSettings.{CurrentEnvironmentName}.json", false, true)
            .AddJsonFile($"appsettings.TuyaManagerSettings.json", false, true)
        ;

        return webApplicationBuilder;
    }

    private static WebApplicationBuilder AddDbContexts(this WebApplicationBuilder webApplicationBuilder)
    {
        Dependencies.AddDbCxtContext(webApplicationBuilder);

        _ = webApplicationBuilder.Services
            .AddDbContext<FuelPrices.Lib.Infrastructure.Data.FuelPricesDbContext>((iServiceProvider, dbContextOptionsBuilder) =>
            {
                string ConnectionStringName = nameof(FuelPrices.Lib.Infrastructure.Data.FuelPricesDbContext);
                string ConnectionString = webApplicationBuilder.Configuration.GetConnectionString($"{ConnectionStringName}") ?? throw new KeyNotFoundException($"Connection string '{ConnectionStringName}' not found.");
                string FullFilePath = Path.GetFullPath(
                    ConnectionString[DatabaseStrings.DataSource.Length..],
                    System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (!File.Exists(FullFilePath))
                    throw new FileNotFoundException("Database file not found.", FullFilePath);

                _ = dbContextOptionsBuilder.UseSqlite($"{DatabaseStrings.DataSource}{FullFilePath}");
                dbContextOptionsBuilder.ConfigureDebugOptions();
            }
            , ServiceLifetime.Singleton
            , ServiceLifetime.Singleton);

        return webApplicationBuilder;
    }

    private static WebApplicationBuilder AddMyServices(this WebApplicationBuilder webApplicationBuilder)
    {
        // Add Todo service for components adopting SSR
        //_ = webApplicationBuilder.Services.AddScoped<IMovieService, ServerMovieService>();

        webApplicationBuilder.Services.TryAddSingleton(webApplicationBuilder.Configuration.GetSection(nameof(Libs.SmtpService.Settings.SmtpServiceSettings)).Get<Libs.SmtpService.Settings.SmtpServiceSettings>()!);
        webApplicationBuilder.Services.TryAddTransient<Libs.SmtpService.Services.SmtpService>();

        webApplicationBuilder.Services.TryAddSingleton(webApplicationBuilder.Configuration.GetSection(nameof(TelegramSettings)).Get<TelegramSettings>()!);
        webApplicationBuilder.Services.TryAddSingleton<TelegramHostedService>();
        _ = webApplicationBuilder.Services.AddHostedService<TelegramHostedService>();

        _ = webApplicationBuilder.Services.AddHttpClient(nameof(FuelPrices.Lib.Core.Settings.Minetur));
        _ = webApplicationBuilder.Services.AddHostedService<FuelPrices.Lib.Services.ObtainDataCronBackgroundService>();

        webApplicationBuilder.Services.TryAddSingleton(webApplicationBuilder.Configuration.GetSection(nameof(Pvpc.Lib.Settings.PvpcSettings)).Get<Pvpc.Lib.Settings.PvpcSettings>()!);
        webApplicationBuilder.Services.TryAddSingleton(webApplicationBuilder.Configuration.GetSection(nameof(Pvpc.Lib.Settings.TuyaManagerSettings)).Get<Pvpc.Lib.Settings.TuyaManagerSettings>()!);
        webApplicationBuilder.Services.TryAddSingleton<PvpcCronBackgroundService>();
        _ = webApplicationBuilder.Services.AddHostedService<PvpcCronBackgroundService>();
        webApplicationBuilder.Services.TryAddSingleton<TuyaManagerCronBackgroundService>();
        _ = webApplicationBuilder.Services.AddHostedService<TuyaManagerCronBackgroundService>();

        _ = webApplicationBuilder.Services.AddHostedService<OutboxCronBackgroundService>();

        webApplicationBuilder.Services.TryAddSingleton<WebComparer.Lib.Services.WebComparerHostedService>();
        _ = webApplicationBuilder.Services.AddHostedService<WebComparer.Lib.Services.WebComparerHostedService>();

        return webApplicationBuilder;
    }
}
