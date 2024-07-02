using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Seedysoft.Pvpc.Lib.Test;

<<<<<<<< HEAD:test/Pvpc/Lib.Test/PvpcBackgroundServiceCronTests.cs
public sealed class PvpcBackgroundServiceCronTests(PvpcBackgroundServiceCronFixture pvpcServiceFixture)
========
public sealed class PvpcBackgroundServiceCronTest(PvpcBackgroundServiceCronFixture pvpcServiceFixture)
>>>>>>>> 0c2ffd3 (Try to reference Octokit project to see where is the fail):test/Pvpc/Lib.Test/PvpcBackgroundServiceCronTest.cs
    : IClassFixture<PvpcBackgroundServiceCronFixture>
{
    public PvpcBackgroundServiceCronFixture PvpcServiceFixture { get; } = pvpcServiceFixture;

    [Fact]
    public void IsTimeToChargeNoPrices()
    {
        Settings.TuyaManagerSettings tuyaManagerSettings = new()
        {
            CronExpression = Cronos.CronExpression.Hourly.ToString(),
            AllowChargeWhenKWhPriceInEurosIsBelowThan = decimal.MaxValue,
            ChargingHoursPerDay = 4,
        };

        bool res = Services.PvpcBackgroundServiceCron.IsTimeToCharge(
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

        bool res = Services.PvpcBackgroundServiceCron.IsTimeToCharge(
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
        bool res = Services.PvpcBackgroundServiceCron.IsTimeToCharge(
            PvpcServiceFixture.Prices,
            PvpcServiceFixture.TimeToQuery,
            tuyaManagerSettings);

        Assert.Equal(res, PvpcServiceFixture.Prices.Any(x => x.KWhPriceInEuros <= PvpcServiceFixture.MinPriceAllowed));
    }
}

public sealed class PvpcBackgroundServiceCronFixture : Libs.Infrastructure.Test.BaseFixture
{
    public PvpcBackgroundServiceCronFixture()
    {
        Settings.PvpcSettings pvpcSettings = new()
        {
            CronExpression = "* * 30 2 *", // At every minute on day-of-month 30 in February.
            DataUrlTemplate = @"https://apidatos.ree.es/es/datos/mercados/precios-mercados-tiempo-real?start_date={0:yyyy-MM-dd}T00:00&end_date={0:yyyy-MM-dd}T23:59&time_trunc=hour&geo_limit=peninsular",
            PvpcId = "1001",
        };

<<<<<<<< HEAD:test/Pvpc/Lib.Test/PvpcBackgroundServiceCronTests.cs
        IServiceCollection services = new ServiceCollection();
        _ = services
            .AddSingleton(pvpcSettings)
            .AddSingleton(GetDbCxt())
            .AddSingleton<Microsoft.Extensions.Logging.ILogger<Services.PvpcBackgroundServiceCron>>(new NullLogger<Services.PvpcBackgroundServiceCron>());
========
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.TryAddSingleton(pvpcSettings);
        serviceCollection.TryAddSingleton(GetDbCxt());
        serviceCollection.TryAddSingleton<Microsoft.Extensions.Logging.ILogger<Services.PvpcBackgroundServiceCron>>(new NullLogger<Services.PvpcBackgroundServiceCron>());
        serviceCollection.TryAddSingleton<Microsoft.Extensions.Hosting.IHostApplicationLifetime>(new ApplicationLifetime(new NullLogger<ApplicationLifetime>()));
>>>>>>>> 0c2ffd3 (Try to reference Octokit project to see where is the fail):test/Pvpc/Lib.Test/PvpcBackgroundServiceCronTest.cs

        PvpcService = new(
            services.BuildServiceProvider(),
            new ApplicationLifetime(new NullLogger<ApplicationLifetime>()));

        TimeToQuery = DateTimeOffset.UtcNow;
        MinPriceAllowed = 0.05M;
        Prices = Enumerable.Range(0, 24)
            .Select(i => new Libs.Core.Entities.Pvpc(
                TimeToQuery.UtcDateTime.AddHours(i),
                decimal.Divide(Random.Shared.Next(40_000, 220_000), 1_000M)))
            .ToArray();
        Prices.Last(x => x.AtDateTimeOffset <= TimeToQuery).MWhPriceInEuros = 49M; // 0.049 KWhPriceInEuros
    }

    public Services.PvpcBackgroundServiceCron PvpcService { get; }
    public Libs.Core.Entities.Pvpc[] Prices { get; }
    public DateTimeOffset TimeToQuery { get; }
    public decimal MinPriceAllowed { get; }

    public override void Dispose()
    {
        PvpcService?.Dispose();
        base.Dispose();
    }
}
