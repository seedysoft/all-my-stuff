using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Seedysoft.Update.Lib.Settings;

namespace Seedysoft.Update.Lib.Dependencies;

internal sealed class Configurator : Libs.Utils.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder) =>
        _ = hostApplicationBuilder.Configuration.AddJsonFile($"appsettings.{nameof(UpdateSettings)}.json", optional: false, reloadOnChange: true);

    protected override void AddDbContexts(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder) { /* No DbContexts */ }

    protected override void AddMyServices(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder) =>
        hostApplicationBuilder.Services.TryAddSingleton(hostApplicationBuilder.Configuration.GetSection(nameof(UpdateSettings)).Get<UpdateSettings>()!);
}
