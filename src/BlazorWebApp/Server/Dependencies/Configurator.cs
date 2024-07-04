using MudBlazor.Services;

namespace Seedysoft.BlazorWebApp.Server.Dependencies;

internal sealed class Configurator : Libs.Utils.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(IHostApplicationBuilder hostApplicationBuilder)
    {
        string CurrentEnvironmentName = hostApplicationBuilder.Environment.EnvironmentName;

        _ = hostApplicationBuilder.Configuration
            .AddJsonFile($"appsettings.BlazorWebApp.Server.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.BlazorWebApp.Server.{CurrentEnvironmentName}.json", optional: false, reloadOnChange: true);
    }

    protected override void AddDbContexts(IHostApplicationBuilder hostApplicationBuilder) { /* No DbContexts */ }

    protected override void AddMyServices(IHostApplicationBuilder hostApplicationBuilder)
    {
        // TODO   Add service for components adopting SSR
        //_ = hostApplicationBuilder.Services.AddScoped<IMovieService, ServerMovieService>();

        _ = hostApplicationBuilder.Services.AddHostedService<Libs.Update.UpdateService>();

        _ = hostApplicationBuilder.Services.AddHostedService<Libs.TelegramBot.Services.TelegramHostedService>();

        _ = hostApplicationBuilder.Services.AddHostedService<Outbox.Lib.Services.OutboxBackgroundServiceCron>();

        _ = hostApplicationBuilder.Services.AddHostedService<Pvpc.Lib.Services.PvpcBackgroundServiceCron>();
        _ = hostApplicationBuilder.Services.AddHostedService<Pvpc.Lib.Services.TuyaManagerBackgroundServiceCron>();

        _ = hostApplicationBuilder.Services.AddHostedService<WebComparer.Lib.Services.WebComparerCronBackgroundService>();

        // Add services to the container.
        _ = hostApplicationBuilder.Services
            .AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        _ = hostApplicationBuilder.Services
            .AddSystemd()

            .AddMudServices()

            .AddHttpClient() // Needed for server rendering

            .AddControllers()
        ;

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        _ = hostApplicationBuilder.Services
            .AddEndpointsApiExplorer()
            .AddOpenApiDocument()
        ;
    }
}
