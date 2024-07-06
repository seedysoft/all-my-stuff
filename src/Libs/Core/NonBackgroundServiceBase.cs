namespace Seedysoft.Libs.Core;

public abstract class NonBackgroundServiceBase(IServiceProvider serviceProvider)
{
    protected IServiceProvider ServiceProvider { get; init; } = serviceProvider;
}
