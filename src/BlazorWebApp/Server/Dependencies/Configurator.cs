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

        //        hostApplicationBuilder.Services.TryAddSingleton(hostApplicationBuilder.Configuration.GetSection(nameof(SmtpServiceLib.Settings.SmtpServiceSettings)).Get<SmtpServiceLib.Settings.SmtpServiceSettings>()!);
        //        hostApplicationBuilder.Services.TryAddTransient<SmtpServiceLib.Services.SmtpService>();

        //        hostApplicationBuilder.Services.TryAddSingleton(hostApplicationBuilder.Configuration.GetSection(nameof(TelegramLib.Settings.TelegramSettings)).Get<TelegramLib.Settings.TelegramSettings>()!);
        //        hostApplicationBuilder.Services.TryAddSingleton<TelegramLib.Services.TelegramHostedService>();
        //        _ = hostApplicationBuilder.Services.AddHostedService<TelegramLib.Services.TelegramHostedService>();

        //        _ = hostApplicationBuilder.Services.AddHttpClient(nameof(Carburantes.CoreLib.Settings.Minetur));
        //        _ = hostApplicationBuilder.Services.AddHostedService<Seedysoft.CarburantesLib.Services.ObtainDataCronBackgroundService>();

        //        hostApplicationBuilder.Services.TryAddSingleton(hostApplicationBuilder.Configuration.GetSection(nameof(PvpcLib.Settings.PvpcSettings)).Get<PvpcLib.Settings.PvpcSettings>()!);
        //        hostApplicationBuilder.Services.TryAddSingleton(hostApplicationBuilder.Configuration.GetSection(nameof(PvpcLib.Settings.TuyaManagerSettings)).Get<PvpcLib.Settings.TuyaManagerSettings>()!);
        //        hostApplicationBuilder.Services.TryAddSingleton<PvpcLib.Services.PvpcCronBackgroundService>();
        //        _ = hostApplicationBuilder.Services.AddHostedService<PvpcLib.Services.PvpcCronBackgroundService>();
        //        hostApplicationBuilder.Services.TryAddSingleton<PvpcLib.Services.TuyaManagerCronBackgroundService>();
        //        _ = hostApplicationBuilder.Services.AddHostedService<PvpcLib.Services.TuyaManagerCronBackgroundService>();

        //        _ = hostApplicationBuilder.Services.AddHostedService<OutboxLib.Services.OutboxCronBackgroundService>();

        //        hostApplicationBuilder.Services.TryAddSingleton<WebComparerLib.Services.WebComparerHostedService>();
        //        _ = hostApplicationBuilder.Services.AddHostedService<WebComparerLib.Services.WebComparerHostedService>();

        //        return hostApplicationBuilder;
    }
}
