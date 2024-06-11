using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Seedysoft.InfrastructureLib.Extensions;

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
        webApplication.Services.GetRequiredService<Carburantes.Infrastructure.Data.CarburantesDbContext>().Database.Migrate();

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

            .AddJsonFile($"appsettings.CarburantesConnectionStrings.{CurrentEnvironmentName}.json", false, true)

            .AddJsonFile($"appsettings.Carburantes.Infrastructure.json", false, true)
            .AddJsonFile($"appsettings.Carburantes.Infrastructure.{CurrentEnvironmentName}.json", false, true)

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
        InfrastructureLib.Dependencies.AddDbCxtContext(webApplicationBuilder);

        _ = webApplicationBuilder.Services
            .AddDbContext<Carburantes.Infrastructure.Data.CarburantesDbContext>((iServiceProvider, dbContextOptionsBuilder) =>
            {
                string ConnectionStringName = nameof(Carburantes.Infrastructure.Data.CarburantesDbContext);
                string ConnectionString = webApplicationBuilder.Configuration.GetConnectionString($"{ConnectionStringName}") ?? throw new KeyNotFoundException($"Connection string '{ConnectionStringName}' not found.");
                string FullFilePath = Path.GetFullPath(ConnectionString, System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (!File.Exists(FullFilePath))
                    throw new FileNotFoundException("Database file not found.", FullFilePath);

                _ = dbContextOptionsBuilder.UseSqlite($"{CoreLib.Constants.DatabaseStrings.DataSource}{FullFilePath}");
                dbContextOptionsBuilder.ConfigureDebugOptions();
            }
            , ServiceLifetime.Transient
            , ServiceLifetime.Transient)

            ;

        return webApplicationBuilder;
    }

    private static WebApplicationBuilder AddMyServices(this WebApplicationBuilder webApplicationBuilder)
    {
        // Add Todo service for components adopting SSR
        //_ = webApplicationBuilder.Services.AddScoped<IMovieService, ServerMovieService>();

        webApplicationBuilder.Services.TryAddSingleton(webApplicationBuilder.Configuration.GetSection(nameof(SmtpServiceLib.Settings.SmtpServiceSettings)).Get<SmtpServiceLib.Settings.SmtpServiceSettings>()!);
        webApplicationBuilder.Services.TryAddTransient<SmtpServiceLib.Services.SmtpService>();

        webApplicationBuilder.Services.TryAddSingleton(webApplicationBuilder.Configuration.GetSection(nameof(TelegramLib.Settings.TelegramSettings)).Get<TelegramLib.Settings.TelegramSettings>()!);
        webApplicationBuilder.Services.TryAddSingleton<TelegramLib.Services.TelegramHostedService>();
        _ = webApplicationBuilder.Services.AddHostedService<TelegramLib.Services.TelegramHostedService>();

        _ = webApplicationBuilder.Services.AddHttpClient(nameof(Carburantes.Core.Settings.Minetur));
        _ = webApplicationBuilder.Services.AddHostedService<Carburantes.Services.ObtainDataCronBackgroundService>();

        webApplicationBuilder.Services.TryAddSingleton(webApplicationBuilder.Configuration.GetSection(nameof(PvpcLib.Settings.PvpcSettings)).Get<PvpcLib.Settings.PvpcSettings>()!);
        webApplicationBuilder.Services.TryAddSingleton(webApplicationBuilder.Configuration.GetSection(nameof(PvpcLib.Settings.TuyaManagerSettings)).Get<PvpcLib.Settings.TuyaManagerSettings>()!);
        webApplicationBuilder.Services.TryAddSingleton<PvpcLib.Services.PvpcCronBackgroundService>();
        _ = webApplicationBuilder.Services.AddHostedService<PvpcLib.Services.PvpcCronBackgroundService>();
        webApplicationBuilder.Services.TryAddSingleton<PvpcLib.Services.TuyaManagerCronBackgroundService>();
        _ = webApplicationBuilder.Services.AddHostedService<PvpcLib.Services.TuyaManagerCronBackgroundService>();

        _ = webApplicationBuilder.Services.AddHostedService<OutboxLib.Services.OutboxCronBackgroundService>();

        webApplicationBuilder.Services.TryAddSingleton<WebComparerLib.Services.WebComparerHostedService>();
        _ = webApplicationBuilder.Services.AddHostedService<WebComparerLib.Services.WebComparerHostedService>();

        return webApplicationBuilder;
    }
}
