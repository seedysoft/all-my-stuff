using Seedysoft.PvpcLib.Services;

namespace Seedysoft.PvpcLib.Test;

public sealed class TuyaManagerCronBackgroundServiceTests(TuyaManagerCronBackgroundServiceFixture tuyaServiceFixture)
    : IClassFixture<TuyaManagerCronBackgroundServiceFixture>
{
    public TuyaManagerCronBackgroundServiceFixture TuyaServiceFixture { get; } = tuyaServiceFixture;

    [Fact]
    public async Task DoWorkAsyncTest() => await TuyaServiceFixture.TuyaManagerService.DoWorkAsync(default);
}

public sealed class TuyaManagerCronBackgroundServiceFixture : InfrastructureLib.Test.BaseFixture
{
    public TuyaManagerCronBackgroundServiceFixture()
    {
        Settings.TuyaManagerSettings tuyaManagerSettings = new()
        {
            CronExpression = "* * 30 2 *", // At every minute on day-of-month 30 in February.
        };
        
        InfrastructureLib.DbContexts.DbCxt dbCxt = GetDbCxt();

        Microsoft.Extensions.Logging.ILogger<TuyaManagerCronBackgroundService> logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<TuyaManagerCronBackgroundService>();
        
        TuyaManagerService = new(tuyaManagerSettings, dbCxt, logger);
    }

    public TuyaManagerCronBackgroundService TuyaManagerService { get; }

    public void Dispose() => TuyaManagerService.Dispose();
}
