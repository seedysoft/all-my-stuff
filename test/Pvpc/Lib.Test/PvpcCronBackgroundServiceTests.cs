using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Seedysoft.Pvpc.Lib.Test;

public sealed class PvpcCronBackgroundServiceTests(PvpcCronBackgroundServiceFixture pvpcServiceFixture)
    : IClassFixture<PvpcCronBackgroundServiceFixture>
{
    public PvpcCronBackgroundServiceFixture PvpcServiceFixture { get; } = pvpcServiceFixture;

    [Fact]
    public void IsTimeToChargeNoPrices()
    {
        Settings.TuyaManagerSettings tuyaManagerSettings = new()
        {
            CronExpression = Cronos.CronExpression.Hourly.ToString(),
            AllowChargeWhenKWhPriceInEurosIsBelowThan = decimal.MaxValue,
            ChargingHoursPerDay = 4,
        };

        bool res = Services.PvpcCronBackgroundService.IsTimeToCharge(
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
            CronExpression = Cronos.CronExpression.Hourly.ToString(),
            AllowChargeWhenKWhPriceInEurosIsBelowThan = decimal.MaxValue,
            ChargingHoursPerDay = 4,
        };

        bool res = Services.PvpcCronBackgroundService.IsTimeToCharge(
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
            CronExpression = Cronos.CronExpression.Hourly.ToString(),
            AllowChargeWhenKWhPriceInEurosIsBelowThan = PvpcServiceFixture.MinPriceAllowed,
            ChargingHoursPerDay = 4,
        };
        bool res = Services.PvpcCronBackgroundService.IsTimeToCharge(
            PvpcServiceFixture.Prices,
            PvpcServiceFixture.TimeToQuery,
            tuyaManagerSettings);

        Assert.Equal(res, PvpcServiceFixture.Prices.Any(x => x.KWhPriceInEuros <= PvpcServiceFixture.MinPriceAllowed));
    }
}

public sealed class PvpcCronBackgroundServiceFixture : Libs.Infrastructure.Test.BaseFixture
{
    public PvpcCronBackgroundServiceFixture()
    {
        Settings.PvpcSettings pvpcSettings = new()
        {
            CronExpression = "* * 30 2 *", // At every minute on day-of-month 30 in February.
            DataUrlTemplate = @"https://apidatos.ree.es/es/datos/mercados/precios-mercados-tiempo-real?start_date={0:yyyy-MM-dd}T00:00&end_date={0:yyyy-MM-dd}T23:59&time_trunc=hour&geo_limit=peninsular",
            PvpcId = "1001",
        };

        PvpcService = new(
            pvpcSettings
            , GetDbCxt()
            , new NullLogger<Services.PvpcCronBackgroundService>()
            , new ApplicationLifetime(new NullLogger<ApplicationLifetime>()));

        TimeToQuery = DateTimeOffset.UtcNow;
        MinPriceAllowed = 0.05M;
        Prices = Enumerable.Range(0, 24)
            .Select(i => new Libs.Core.Entities.Pvpc(
                TimeToQuery.UtcDateTime.AddHours(i),
                decimal.Divide(Random.Shared.Next(40_000, 220_000), 1_000M)))
            .ToArray();
        Prices.Last(x => x.AtDateTimeOffset <= TimeToQuery).MWhPriceInEuros = 49M; // 0.049 KWhPriceInEuros
    }

    public Services.PvpcCronBackgroundService PvpcService { get; }
    public Libs.Core.Entities.Pvpc[] Prices { get; }
    public DateTimeOffset TimeToQuery { get; }
    public decimal MinPriceAllowed { get; }

    public override void Dispose()
    {
        PvpcService?.Dispose();
        base.Dispose();
    }
}
