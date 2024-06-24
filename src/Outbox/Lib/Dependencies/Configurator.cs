using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Seedysoft.Outbox.Lib.Dependencies;

internal sealed class Configurator : Libs.Utils.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(IHostApplicationBuilder hostApplicationBuilder) { /* No JsonFiles */ }

    protected override void AddDbContexts(IHostApplicationBuilder hostApplicationBuilder) { /* No DbContexts */ }

    protected override void AddMyServices(IHostApplicationBuilder hostApplicationBuilder)
        => hostApplicationBuilder.Services.TryAddSingleton<Services.OutboxCronBackgroundService>();
}
// _ = builder.Configuration
//     .AddJsonFile($"appsettings.SmtpServiceSettings.json", false, true)
//     .AddJsonFile($"appsettings.TelegramSettings.json", false, true)
//     .AddJsonFile($"appsettings.TelegramSettings.{builder.Environment.EnvironmentName}.json", false, true);
// 
// builder.Services.TryAddSingleton(builder.Configuration.GetSection(nameof(Libs.SmtpService.Settings.SmtpServiceSettings)).Get<Libs.SmtpService.Settings.SmtpServiceSettings>()!);
// builder.Services.TryAddTransient<Libs.SmtpService.Services.SmtpService>();
// 
// builder.Services.TryAddSingleton(builder.Configuration.GetSection(nameof(Libs.Telegram.Settings.TelegramSettings)).Get<Libs.Telegram.Settings.TelegramSettings>()!);
// builder.Services.TryAddSingleton<Libs.Telegram.Services.TelegramHostedService>();
// 
// builder.Services.TryAddSingleton<Lib.Services.OutboxCronBackgroundService>();
// 
// IHost host = builder.Build();
