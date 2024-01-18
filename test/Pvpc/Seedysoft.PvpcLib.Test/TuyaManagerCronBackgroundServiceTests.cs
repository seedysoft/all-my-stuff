using Microsoft.EntityFrameworkCore;
using Seedysoft.PvpcLib.Services;

namespace Seedysoft.PvpcLib.Test;

public sealed class TuyaManagerCronBackgroundServiceTests(TuyaManagerCronBackgroundServiceFixture tuyaServiceFixture)
    : IClassFixture<TuyaManagerCronBackgroundServiceFixture>
{
    public TuyaManagerCronBackgroundServiceFixture TuyaServiceFixture { get; } = tuyaServiceFixture;

    [Fact]
    public async void DoWorkAsyncTest() => await TuyaServiceFixture.TuyaManagerService.DoWorkAsync(default);
}

public sealed class TuyaManagerCronBackgroundServiceFixture : IDisposable
{
    public TuyaManagerCronBackgroundServiceFixture()
    {
        Settings.TuyaManagerSettings tuyaManagerSettings = new()
        {
            CronExpression = "* * 30 2 *", // At every minute on day-of-month 30 in February.
        };
        DbContextOptions<InfrastructureLib.DbContexts.DbCxt> options = new();
        InfrastructureLib.DbContexts.DbCxt dbCxt = new(options);
        dbCxt.Database.Migrate();
        Microsoft.Extensions.Logging.ILogger<TuyaManagerCronBackgroundService> logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<TuyaManagerCronBackgroundService>();
        TuyaManagerService = new(tuyaManagerSettings, dbCxt, logger);
    }

    public TuyaManagerCronBackgroundService TuyaManagerService { get; }

    public void Dispose() => TuyaManagerService.Dispose();
}
