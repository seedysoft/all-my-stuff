using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Seedysoft.Pvpc.Lib.Test;

public sealed class TuyaManagerCronBackgroundServiceTests(TuyaManagerCronBackgroundServiceFixture tuyaServiceFixture)
    : IClassFixture<TuyaManagerCronBackgroundServiceFixture>
{
    public TuyaManagerCronBackgroundServiceFixture TuyaServiceFixture { get; } = tuyaServiceFixture;

    [Fact]
    public async Task DoWorkAsyncTest() => await TuyaServiceFixture.TuyaManagerService.DoWorkAsync(default);
}

public sealed class TuyaManagerCronBackgroundServiceFixture : Libs.Infrastructure.Test.BaseFixture
{
    public TuyaManagerCronBackgroundServiceFixture()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.TryAddSingleton(new Settings.TuyaManagerSettings() { CronExpression = "* * 30 2 *", /* At every minute on day-of-month 30 in February. */         });
        serviceCollection.TryAddSingleton(GetDbCxt());
        serviceCollection.TryAddSingleton(new NullLogger<Services.TuyaManagerBackgroundServiceCron>());
        serviceCollection.TryAddSingleton(new ApplicationLifetime(new NullLogger<ApplicationLifetime>()));

        TuyaManagerService = new(serviceCollection.BuildServiceProvider());
    }

    public Services.TuyaManagerBackgroundServiceCron TuyaManagerService { get; }

    public override void Dispose()
    {
        TuyaManagerService.Dispose();
        base.Dispose();
    }
}
