using Microsoft.EntityFrameworkCore;
using Seedysoft.PvpcLib.Services;

namespace Seedysoft.PvpcLib.Test;

public sealed class PvpcCronBackgroundServiceTests(PvpcCronBackgroundServiceFixture pvpcServiceFixture)
    : IClassFixture<PvpcCronBackgroundServiceFixture>
{
    public PvpcCronBackgroundServiceFixture PvpcServiceFixture { get; } = pvpcServiceFixture;

    [Fact]
    public void IsTimeToChargeNoPrices()
    {
        bool res = PvpcCronBackgroundService.IsTimeToCharge(
            [],
            PvpcServiceFixture.TimeToQuery,
            decimal.MaxValue);

        Assert.False(res);
    }

    [Fact]
    public void IsTimeToChargeAllowBelowDecimalMaxValue()
    {
        bool res = PvpcCronBackgroundService.IsTimeToCharge(
            PvpcServiceFixture.Prices,
            PvpcServiceFixture.TimeToQuery,
            decimal.MaxValue);

        Assert.True(res);
    }

    [Fact]
    public void IsTimeToChargeIfAnyPriceBelow()
    {
        bool res = PvpcCronBackgroundService.IsTimeToCharge(
            PvpcServiceFixture.Prices,
            PvpcServiceFixture.TimeToQuery,
            PvpcServiceFixture.MinPriceAllowed);

        Assert.Equal(res, PvpcServiceFixture.Prices.Any(x => x.KWhPriceInEuros <= PvpcServiceFixture.MinPriceAllowed));
    }
}

public sealed class PvpcCronBackgroundServiceFixture : IDisposable
{
    public PvpcCronBackgroundServiceFixture()
    {
        Settings.PvpcSettings pvpcSettings = new()
        {
            CronExpression = "* * 30 2 *", // At every minute on day-of-month 30 in February.
        };
        DbContextOptions<InfrastructureLib.DbContexts.DbCxt> options = new();
        InfrastructureLib.DbContexts.DbCxt dbCxt = new(options);
        dbCxt.Database.Migrate();
        Microsoft.Extensions.Logging.ILogger<PvpcCronBackgroundService> logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<PvpcCronBackgroundService>();
        PvpcService = new(pvpcSettings, dbCxt, logger);

        TimeToQuery = DateTimeOffset.UtcNow;
        MinPriceAllowed = 0.05M;
        Prices =
            Enumerable.Range(0, 24)
            .Select(i => new CoreLib.Entities.Pvpc(
                TimeToQuery.UtcDateTime.AddHours(i),
                decimal.Divide(Random.Shared.Next(40_000, 220_000), 1_000M)))
            .ToArray();
    }

    public PvpcCronBackgroundService PvpcService { get; }
    public CoreLib.Entities.Pvpc[] Prices { get; }
    public DateTimeOffset TimeToQuery { get; }
    public decimal MinPriceAllowed { get; }

    public void Dispose() => PvpcService?.Dispose();
}
