using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Seedysoft.Pvpc.Lib.Test;

public sealed class TuyaManagerBackgroundServiceCronTests(TuyaManagerBackgroundServiceCronFixture tuyaServiceFixture)
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
        Settings.TuyaManagerSettings tuyaManagerSettings = new()
        {
            CronExpression = "* * 30 2 *", // At every minute on day-of-month 30 in February.
        };

        IServiceCollection services = new ServiceCollection();
        _ = services
            .AddSingleton(tuyaManagerSettings)
            .AddSingleton(GetDbCxt())
            .AddSingleton<Microsoft.Extensions.Logging.ILogger<Services.TuyaManagerBackgroundServiceCron>>(new NullLogger<Services.TuyaManagerBackgroundServiceCron>());

        TuyaManagerService = new(
            services.BuildServiceProvider(),
            new ApplicationLifetime(new NullLogger<ApplicationLifetime>()));
    }

    public Services.TuyaManagerBackgroundServiceCron TuyaManagerService { get; }

    public override void Dispose()
    {
        TuyaManagerService.Dispose();
        base.Dispose();
    }
}
