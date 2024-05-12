using Seedysoft.PvpcLib.Services;

namespace Seedysoft.PvpcLib.Test;

public sealed class PvpcCronBackgroundServiceTests(PvpcCronBackgroundServiceFixture pvpcServiceFixture)
    : IClassFixture<PvpcCronBackgroundServiceFixture>
{
    public PvpcCronBackgroundServiceFixture PvpcServiceFixture { get; } = pvpcServiceFixture;

    [Fact]
    public void IsTimeToChargeNoPrices()
    {
        Settings.TuyaManagerSettings tuyaManagerSettings = new()
        {
            AllowChargeWhenKWhPriceInEurosIsBelowThan = decimal.MaxValue,
            ChargingHoursPerDay = 4,
        };

        bool res = PvpcCronBackgroundService.IsTimeToCharge(
            [],
            PvpcServiceFixture.TimeToQuery,
            tuyaManagerSettings);

        Assert.False(res);
    }

    [Fact]
    public void IsTimeToChargeAllowBelowDecimalMaxValue()
    {
        Settings.TuyaManagerSettings tuyaManagerSettings = new()
        {
            AllowChargeWhenKWhPriceInEurosIsBelowThan = decimal.MaxValue,
            ChargingHoursPerDay = 4,
        };

        bool res = PvpcCronBackgroundService.IsTimeToCharge(
            PvpcServiceFixture.Prices,
            PvpcServiceFixture.TimeToQuery,
            tuyaManagerSettings);

        Assert.True(res);
    }

    [Fact]
    public void IsTimeToChargeIfAnyPriceBelow()
    {
        Settings.TuyaManagerSettings tuyaManagerSettings = new()
        {
            AllowChargeWhenKWhPriceInEurosIsBelowThan = PvpcServiceFixture.MinPriceAllowed,
            ChargingHoursPerDay = 4,
        };
        bool res = PvpcCronBackgroundService.IsTimeToCharge(
            PvpcServiceFixture.Prices,
            PvpcServiceFixture.TimeToQuery,
            tuyaManagerSettings);

        Assert.Equal(res, PvpcServiceFixture.Prices.Any(x => x.KWhPriceInEuros <= PvpcServiceFixture.MinPriceAllowed));
    }
}

public sealed class PvpcCronBackgroundServiceFixture : InfrastructureLib.Test.BaseFixture
{
    public PvpcCronBackgroundServiceFixture()
    {
        Settings.PvpcSettings pvpcSettings = new()
        {
            CronExpression = "* * 30 2 *", // At every minute on day-of-month 30 in February.
        };

        InfrastructureLib.DbContexts.DbCxt dbCxt = GetDbCxt();

        Microsoft.Extensions.Logging.Abstractions.NullLogger<PvpcCronBackgroundService> logger = new();

        PvpcService = new(pvpcSettings, dbCxt, logger);

        TimeToQuery = DateTimeOffset.UtcNow;
        MinPriceAllowed = 0.05M;
        Prices = Enumerable.Range(0, 24)
            .Select(i => new CoreLib.Entities.Pvpc(
                TimeToQuery.UtcDateTime.AddHours(i),
                decimal.Divide(Random.Shared.Next(40_000, 220_000), 1_000M)))
            .ToArray();
        Prices.Last(x => x.AtDateTimeOffset <= TimeToQuery).MWhPriceInEuros = 49M; // 0.049 KWhPriceInEuros
    }

    public PvpcCronBackgroundService PvpcService { get; }
    public CoreLib.Entities.Pvpc[] Prices { get; }
    public DateTimeOffset TimeToQuery { get; }
    public decimal MinPriceAllowed { get; }

    public void Dispose() => PvpcService?.Dispose();
}
