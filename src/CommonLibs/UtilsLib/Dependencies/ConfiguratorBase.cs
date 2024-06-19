using Microsoft.Extensions.Hosting;

namespace Seedysoft.UtilsLib.Dependencies;

public abstract class ConfiguratorBase
{
    public void AddDependencies(IHostApplicationBuilder hostApplicationBuilder)
    {
        AddJsonFiles(hostApplicationBuilder);
        AddDbContexts(hostApplicationBuilder);
        AddMyServices(hostApplicationBuilder);
    }

    protected abstract void AddJsonFiles(IHostApplicationBuilder hostApplicationBuilder);

    protected abstract void AddDbContexts(IHostApplicationBuilder hostApplicationBuilder);

    protected abstract void AddMyServices(IHostApplicationBuilder hostApplicationBuilder);
}
