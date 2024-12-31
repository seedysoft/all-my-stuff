using MudBlazor.Services;

namespace Seedysoft.BlazorWebApp.Server.Dependencies;

public sealed class Configurator : Libs.Core.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(IHostApplicationBuilder hostApplicationBuilder)
    {
        string CurrentEnvironmentName = hostApplicationBuilder.Environment.EnvironmentName;

        _ = hostApplicationBuilder.Configuration
            .AddJsonFile($"appsettings.{nameof(BlazorWebApp)}.{nameof(Server)}.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{nameof(BlazorWebApp)}.{nameof(Server)}.{CurrentEnvironmentName}.json", optional: false, reloadOnChange: true);
    }

    protected override void AddDbContexts(IHostApplicationBuilder hostApplicationBuilder) { /* No DbContexts */ }

    protected override void AddMyServices(IHostApplicationBuilder hostApplicationBuilder)
    {
        // Add Todo service for components adopting SSR
        //_ = hostApplicationBuilder.Services.AddScoped<IMovieService, ServerMovieService>();

#if !DEBUG
        _ = hostApplicationBuilder.Services.AddHostedService<Libs.TelegramBot.Services.TelegramHostedService>();

        _ = hostApplicationBuilder.Services.AddHostedService<Outbox.Lib.Services.OutboxCronBackgroundService>();

        _ = hostApplicationBuilder.Services.AddHostedService<Pvpc.Lib.Services.PvpcCronBackgroundService>();
        _ = hostApplicationBuilder.Services.AddHostedService<Pvpc.Lib.Services.TuyaManagerCronBackgroundService>();

        _ = hostApplicationBuilder.Services.AddHostedService<WebComparer.Lib.Services.WebComparerCronBackgroundService>();
#endif

        // Add services to the container.
        _ = hostApplicationBuilder.Services
            .AddRazorComponents(static razorComponentsServiceOptions => { })
            .AddInteractiveServerComponents(static circuitOptions => { })
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
