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
