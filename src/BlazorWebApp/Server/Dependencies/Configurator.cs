using Microsoft.AspNetCore.RateLimiting;
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

        //if (System.Diagnostics.Debugger.IsAttached)
        //    System.Diagnostics.Debugger.Break();

        Settings.BlazorWebAppServerSettings BlazorWebAppServerSettings = hostApplicationBuilder.Configuration
            .GetSection(nameof(Settings.BlazorWebAppServerSettings)).Get<Settings.BlazorWebAppServerSettings>()!;
        // Uncomment if needed outside here
        // hostApplicationBuilder.Services.TryAddSingleton(BlazorWebAppServerSettings);

        if (BlazorWebAppServerSettings.UseOutbox)
            _ = hostApplicationBuilder.Services.AddHostedService<Outbox.Lib.Services.OutboxCronBackgroundService>();

        if (BlazorWebAppServerSettings.UsePvpc)
            _ = hostApplicationBuilder.Services.AddHostedService<Pvpc.Lib.Services.PvpcCronBackgroundService>();

        if (BlazorWebAppServerSettings.UseTelegram)
            _ = hostApplicationBuilder.Services.AddHostedService<Libs.TelegramBot.Services.TelegramHostedService>();

        if (BlazorWebAppServerSettings.UseTuyaManager)
            _ = hostApplicationBuilder.Services.AddHostedService<Pvpc.Lib.Services.TuyaManagerCronBackgroundService>();

        if (BlazorWebAppServerSettings.UseUpdater)
            _ = hostApplicationBuilder.Services.AddHostedService<Libs.Update.Services.UpdaterCronBackgroundService>();

        if (BlazorWebAppServerSettings.UseWebComparer)
            _ = hostApplicationBuilder.Services.AddHostedService<WebComparer.Lib.Services.WebComparerCronBackgroundService>();

        // Add services to the container.
        _ = hostApplicationBuilder.Services
            .AddRazorComponents(razorComponentsServiceOptions => razorComponentsServiceOptions.DetailedErrors = hostApplicationBuilder.Environment.IsDevelopment())
            .AddInteractiveServerComponents(circuitOptions => circuitOptions.DetailedErrors = hostApplicationBuilder.Environment.IsDevelopment())
            .AddInteractiveWebAssemblyComponents()
            ;

        _ = hostApplicationBuilder.Services
            .AddSystemd()

            .AddMudServices()

            .AddHttpClient() // Needed for server rendering
            ;

        _ = hostApplicationBuilder.Services.AddRateLimiter(options =>
        {
            _ = options.AddFixedWindowLimiter("fixed", opt =>
            {
                opt.PermitLimit = 5; // Máximo 5 peticiones
                opt.Window = TimeSpan.FromSeconds(10); // En un intervalo de 10 segundos
                opt.QueueLimit = 2; // Permite encolar hasta 2 peticiones adicionales
            });
        });

        if (hostApplicationBuilder.Environment.IsProduction())
        {
            _ = (hostApplicationBuilder as WebApplicationBuilder)?.WebHost.ConfigureKestrel(options =>
            {
                options.AddServerHeader = false;

                options.ConfigureHttpsDefaults(https => https.SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13);

                options.Limits.MaxRequestBodySize = 5_000_000; // Review request size limits -- the 30 MB default may be larger than needed

                // Consider tightening header limits from the defaults
                options.Limits.MaxRequestHeaderCount = 20; // appropriate for your application

                options.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(130); // balance between resource usage and client experience
                options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30); // shorter values improve slowloris resistance

                // Enable HTTP/2 keep-alive pings (disabled by default)
                options.Limits.Http2.KeepAlivePingDelay = TimeSpan.FromSeconds(30); // enable to detect zombie connections
                options.Limits.Http2.KeepAlivePingTimeout = TimeSpan.FromSeconds(60);
            });
        }
    }
}
