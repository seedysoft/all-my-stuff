namespace Seedysoft.Libs.Core.Dependencies;

public abstract class ConfiguratorBase
{
    public void AddDependencies(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder)
    {
        AddJsonFiles(hostApplicationBuilder);
        AddDbContexts(hostApplicationBuilder);
        AddMyServices(hostApplicationBuilder);
    }

    protected abstract void AddJsonFiles(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder);

    protected abstract void AddDbContexts(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder);

    protected abstract void AddMyServices(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder);
}
