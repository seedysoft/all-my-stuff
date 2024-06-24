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

        Microsoft.Extensions.Logging.ILogger<Services.TuyaManagerCronBackgroundService> logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<Services.TuyaManagerCronBackgroundService>();

        TuyaManagerService = new(tuyaManagerSettings, dbCxt, logger);
    }

    public Services.TuyaManagerCronBackgroundService TuyaManagerService { get; }

    public override void Dispose()
    {
        TuyaManagerService.Dispose();
        base.Dispose();
    }
}
