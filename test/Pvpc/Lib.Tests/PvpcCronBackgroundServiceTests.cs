using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging.Abstractions;

namespace Seedysoft.Pvpc.Lib.Tests;

[TestClass]
public sealed class PvpcCronBackgroundServiceTests : Libs.Infrastructure.Tests.TestClassBase
{
    private static Services.PvpcCronBackgroundService PvpcService = default!;
    private static Libs.Core.Entities.Pvpc[] Prices = default!;
    private static DateTimeOffset TimeToQuery = default!;
    private static decimal MinPriceAllowed = default!;

    [ClassInitialize(InheritanceBehavior.None)]
    public static void ClassInitialize(TestContext testContext)
    {
        Settings.PvpcSettings pvpcSettings = new()
        {
            CronExpression = "* * 30 2 *", // At every minute on day-of-month 30 in February.
            DataUrlTemplate = @"https://apidatos.ree.es/es/datos/mercados/precios-mercados-tiempo-real?start_date={0:yyyy-MM-dd}T00:00&end_date={0:yyyy-MM-dd}T23:59&time_trunc=hour&geo_limit=peninsular",
            PvpcId = "1001",
        };

        IServiceCollection services = new ServiceCollection();
        _ = services
            .AddSingleton(pvpcSettings)
            .AddSingleton(GetDbCxt())
            .AddSingleton<Microsoft.Extensions.Logging.ILogger<Services.PvpcCronBackgroundService>>(new NullLogger<Services.PvpcCronBackgroundService>());

        PvpcService = new(
            services.BuildServiceProvider(),
            new ApplicationLifetime(new NullLogger<ApplicationLifetime>()));

        TimeToQuery = DateTimeOffset.UtcNow;
        MinPriceAllowed = 0.05M;
        Prices = Enumerable.Range(0, 24)
            .Select(static i => new Libs.Core.Entities.Pvpc(
                TimeToQuery.UtcDateTime.AddHours(i),
                decimal.Divide(Random.Shared.Next(40_000, 220_000), 1_000M)))
            .ToArray();
        Prices.Last(static x => x.AtDateTimeOffset <= TimeToQuery).MWhPriceInEuros = 49M; // 0.049 KWhPriceInEuros
    }

    [ClassCleanup(InheritanceBehavior.None, ClassCleanupBehavior.EndOfClass)]
    public static void ClassCleanup() => PvpcService?.Dispose();

    [TestMethod]
    public void IsTimeToChargeNoPricesTest()
    {
        Settings.TuyaManagerSettings tuyaManagerSettings = new()
        {
            CronExpression = Cronos.CronExpression.Hourly.ToString(),
            AllowChargeWhenKWhPriceInEurosIsBelowThan = decimal.MaxValue,
            ChargingHoursPerDay = 4,
        };

        bool res = Services.PvpcCronBackgroundService.IsTimeToCharge(
            [],
            TimeToQuery,
            tuyaManagerSettings);

        Assert.IsFalse(res);
    }

    [TestMethod]
    public void IsTimeToChargeAllowBelowDecimalMaxValueTest()
    {
        Settings.TuyaManagerSettings tuyaManagerSettings = new()
        {
            CronExpression = Cronos.CronExpression.Hourly.ToString(),
            AllowChargeWhenKWhPriceInEurosIsBelowThan = decimal.MaxValue,
            ChargingHoursPerDay = 4,
        };

        bool res = Services.PvpcCronBackgroundService.IsTimeToCharge(
            Prices,
            TimeToQuery,
            tuyaManagerSettings);

        Assert.IsTrue(res);
    }

    [TestMethod]
    public void IsTimeToChargeIfAnyPriceBelowTest()
    {
        Settings.TuyaManagerSettings tuyaManagerSettings = new()
        {
            CronExpression = Cronos.CronExpression.Hourly.ToString(),
            AllowChargeWhenKWhPriceInEurosIsBelowThan = MinPriceAllowed,
            ChargingHoursPerDay = 4,
        };
        bool res = Services.PvpcCronBackgroundService.IsTimeToCharge(
            Prices,
            TimeToQuery,
            tuyaManagerSettings);

        Assert.AreEqual(res, Prices.Any(static x => x.KWhPriceInEuros <= MinPriceAllowed));
    }
}
