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
        Settings.TuyaManagerSettings tuyaManagerSettings = new()
        {
            CronExpression = "* * 30 2 *", // At every minute on day-of-month 30 in February.
        };

        Libs.Infrastructure.DbContexts.DbCxt dbCxt = GetDbCxt();

        TuyaManagerService = new(
            tuyaManagerSettings
            , dbCxt
            , new NullLogger<Services.TuyaManagerCronBackgroundService>()
            , new ApplicationLifetime(new NullLogger<ApplicationLifetime>()));
    }

    public Services.TuyaManagerCronBackgroundService TuyaManagerService { get; }

    public override void Dispose()
    {
        TuyaManagerService.Dispose();
        base.Dispose();
    }
}
