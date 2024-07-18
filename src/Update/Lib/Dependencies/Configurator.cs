﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Seedysoft.Update.Lib.Dependencies;

internal sealed class Configurator : Libs.Utils.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder) =>
        _ = hostApplicationBuilder.Configuration.AddJsonFile($"appsettings.{nameof(Settings.UpdateSettings)}.json", optional: false, reloadOnChange: true);

    protected override void AddDbContexts(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder) { /* No DbContexts */ }

    protected override void AddMyServices(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder)
    {
        hostApplicationBuilder.Services.TryAddSingleton(hostApplicationBuilder.Configuration.GetSection(nameof(Settings.UpdateSettings)).Get<Settings.UpdateSettings>()!);
        hostApplicationBuilder.Services.TryAddSingleton<Services.UpdateBackgroundServiceCron>();
    }
}
