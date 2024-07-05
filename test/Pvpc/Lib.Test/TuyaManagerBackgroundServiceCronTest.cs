using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Seedysoft.Pvpc.Lib.Test;

public sealed class TuyaManagerBackgroundServiceCronTest(TuyaManagerBackgroundServiceCronFixture tuyaServiceFixture)
    : IClassFixture<TuyaManagerBackgroundServiceCronFixture>
{
    public TuyaManagerBackgroundServiceCronFixture TuyaServiceFixture { get; } = tuyaServiceFixture;

    [Fact]
    public async Task DoWorkAsyncTest() => await TuyaServiceFixture.TuyaManagerService.DoWorkAsync(default);
}

public sealed class TuyaManagerBackgroundServiceCronFixture : Libs.Infrastructure.Test.BaseFixture
{
    public TuyaManagerBackgroundServiceCronFixture()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.TryAddSingleton(new Settings.TuyaManagerSettings() { CronExpression = "* * 30 2 *", /* At every minute on day-of-month 30 in February. */         });
        serviceCollection.TryAddSingleton(GetDbCxt());
        serviceCollection.TryAddSingleton<Microsoft.Extensions.Logging.ILogger<Services.TuyaManagerBackgroundServiceCron>>(new NullLogger<Services.TuyaManagerBackgroundServiceCron>());
        serviceCollection.TryAddSingleton<Microsoft.Extensions.Hosting.IHostApplicationLifetime>(new ApplicationLifetime(new NullLogger<ApplicationLifetime>()));

        TuyaManagerService = new(serviceCollection.BuildServiceProvider());
    }

    public Services.TuyaManagerBackgroundServiceCron TuyaManagerService { get; }

    public override void Dispose()
    {
        TuyaManagerService.Dispose();
        base.Dispose();
    }
}
