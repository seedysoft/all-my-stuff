using Microsoft.Extensions.Hosting;

namespace Seedysoft.Libs.MapRazorClassLibrary.Dependencies;

public sealed class Configurator : Core.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(IHostApplicationBuilder hostApplicationBuilder) { /* No JsonFiles */ }

    protected override void AddDbContexts(IHostApplicationBuilder hostApplicationBuilder) { /* No DbContexts */ }

    protected override void AddMyServices(IHostApplicationBuilder hostApplicationBuilder) { /* No Services */ }
}
