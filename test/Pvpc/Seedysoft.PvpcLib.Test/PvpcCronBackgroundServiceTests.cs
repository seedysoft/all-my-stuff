using Seedysoft.PvpcLib.Services;

namespace Seedysoft.PvpcLib.Test;

public sealed class PvpcCronBackgroundServiceTests(PvpcCronBackgroundServiceFixture pvpcServiceFixture)
    : IClassFixture<PvpcCronBackgroundServiceFixture>
{
    [Fact]
    public void IsTimeToChargeNoPrices()
    {
        bool res = PvpcCronBackgroundService.IsTimeToCharge(
            [],
            pvpcServiceFixture.TimeToQuery,
            decimal.MaxValue);

        Assert.False(res);
    }

    [Fact]
    public void IsTimeToChargeAllowBelowDecimalMaxValue()
    {
        bool res = PvpcCronBackgroundService.IsTimeToCharge(
            pvpcServiceFixture.Prices,
            pvpcServiceFixture.TimeToQuery,
            decimal.MaxValue);

        Assert.True(res);
    }

    [Fact]
    public void IsTimeToChargeIfAnyPriceBelow()
    {
        bool res = PvpcCronBackgroundService.IsTimeToCharge(
            pvpcServiceFixture.Prices,
            pvpcServiceFixture.TimeToQuery,
            pvpcServiceFixture.MinPriceAllowed);

        Assert.Equal(res, pvpcServiceFixture.Prices.Any(x => x.KWhPriceInEuros <= pvpcServiceFixture.MinPriceAllowed));
    }
}

public sealed class PvpcCronBackgroundServiceFixture : IDisposable
{
    public PvpcCronBackgroundServiceFixture()
    {
        // TODO         FAKE DATABASE in memory
        Settings.PvpcSettings pvpcSettings = new()
        {
            CronExpression = "* * 30 2 *", // At every minute on day-of-month 30 in February.
        };
        Microsoft.EntityFrameworkCore.DbContextOptions<InfrastructureLib.DbContexts.DbCxt> options = new();
        InfrastructureLib.DbContexts.DbCxt dbCxt = new(options);
        Microsoft.Extensions.Logging.ILogger<PvpcCronBackgroundService> logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<PvpcCronBackgroundService>();
        PvpcService = new(pvpcSettings, dbCxt, logger);

        TimeToQuery = DateTimeOffset.UtcNow;
        MinPriceAllowed = 0.05M;
        Prices =
            Enumerable.Range(0, 24)
            .Select(i => new CoreLib.Entities.PvpcView()
            {
                AtDateTimeOffset = TimeToQuery.UtcDateTime.AddHours(i),
                AtDateTimeUnix = new DateTimeOffset(TimeToQuery.UtcDateTime.AddHours(i)).ToUnixTimeSeconds(),
                MWhPriceInEuros = decimal.Divide(Random.Shared.Next(40_000, 220_000), 1_000M)
            })
            .ToArray();
    }

    public PvpcCronBackgroundService PvpcService { get; init; }
    public CoreLib.Entities.PvpcView[] Prices { get; init; }
    public DateTimeOffset TimeToQuery { get; init; }
    public decimal MinPriceAllowed { get; init; }

    public void Dispose() => PvpcService?.Dispose();
}
