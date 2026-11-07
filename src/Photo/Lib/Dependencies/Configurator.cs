using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Seedysoft.Photo.Lib.Dependencies;

public sealed class Configurator : Libs.Core.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(IHostApplicationBuilder hostApplicationBuilder)
    {
        string CurrentEnvironmentName = hostApplicationBuilder.Environment.EnvironmentName;

        _ = hostApplicationBuilder.Configuration
            .AddJsonFile($"appsettings.{nameof(Settings.PhotoSettings)}.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{nameof(Settings.PhotoSettings)}.{CurrentEnvironmentName}.json", optional: false, reloadOnChange: true);
    }

    protected override void AddDbContexts(IHostApplicationBuilder hostApplicationBuilder) { /* DbContext in Seedysoft.Libs.Infrastructure.Dependencies */ }

    protected override void AddMyServices(IHostApplicationBuilder hostApplicationBuilder)
        => hostApplicationBuilder.Services.TryAddSingleton<Services.PhotoCronBackgroundService>();
}
