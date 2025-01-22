using Microsoft.Extensions.Hosting;

namespace Seedysoft.BlazorWebApp.Client.Dependencies;

public sealed class Configurator : Libs.Core.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(IHostApplicationBuilder hostApplicationBuilder) { /* No JsonFiles */ }

    protected override void AddDbContexts(IHostApplicationBuilder hostApplicationBuilder) { /* No DbContexts */ }

    protected override void AddMyServices(IHostApplicationBuilder hostApplicationBuilder) { /* No MyServices */ }
}
