using MudBlazor.Services;

namespace Seedysoft.BlazorWebApp.Server.Dependencies;

internal sealed class Configurator : Libs.Utils.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(IHostApplicationBuilder hostApplicationBuilder)
    {
        _ = hostApplicationBuilder.Configuration
            .AddJsonFile($"appsettings.BlazorWebApp.Server.json", false, true)
            .AddJsonFile($"appsettings.BlazorWebApp.Server.{hostApplicationBuilder.Environment.EnvironmentName}.json", false, true);
    }

    protected override void AddDbContexts(IHostApplicationBuilder hostApplicationBuilder) { /* No DbContexts */ }

    protected override void AddMyServices(IHostApplicationBuilder hostApplicationBuilder)
    {
        // Add Todo service for components adopting SSR
        //_ = hostApplicationBuilder.Services.AddScoped<IMovieService, ServerMovieService>();

        _ = hostApplicationBuilder.Services.AddHostedService<Libs.Telegram.Services.TelegramHostedService>();

        _ = hostApplicationBuilder.Services.AddHostedService<Outbox.Lib.Services.OutboxCronBackgroundService>();

        _ = hostApplicationBuilder.Services.AddHostedService<Pvpc.Lib.Services.PvpcCronBackgroundService>();
        _ = hostApplicationBuilder.Services.AddHostedService<Pvpc.Lib.Services.TuyaManagerCronBackgroundService>();

        _ = hostApplicationBuilder.Services.AddHostedService<WebComparer.Lib.Services.WebComparerHostedService>();

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
