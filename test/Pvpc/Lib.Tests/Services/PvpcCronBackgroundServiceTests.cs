using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Seedysoft.Pvpc.Lib.Tests.Services;

public sealed class PvpcCronBackgroundServiceTests : Libs.Infrastructure.Tests.TestClassBase, IDisposable
{
    private readonly Lib.Services.PvpcCronBackgroundService PvpcService = default!;
    private readonly Libs.Core.Entities.Pvpc[] Prices = default!;
    private readonly DateTimeOffset TimeToQuery = default!;
    private readonly decimal MinPriceAllowed = default!;
    private bool disposedValue;

    public PvpcCronBackgroundServiceTests(Xunit.Abstractions.ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        Settings.PvpcSettings pvpcSettings = new()
        {
            CronExpression = "* * 30 2 *", // At every minute on day-of-month 30 in February.
            DataUrlTemplate = @"https://apidatos.ree.es/es/datos/mercados/precios-mercados-tiempo-real?start_date={0:yyyy-MM-dd}T00:00&end_date={0:yyyy-MM-dd}T23:59&time_trunc=hour&geo_limit=peninsular",
            PvpcId = "1001",
        };

        // TODO                     Maybe a Console logger????
        IServiceCollection services = new ServiceCollection();
        _ = services
            .AddSingleton(pvpcSettings)
            .AddSingleton<Microsoft.Extensions.Logging.ILogger<Lib.Services.PvpcCronBackgroundService>>(new NullLogger<Lib.Services.PvpcCronBackgroundService>());

        //AddDbContext(services);

        //PvpcService = new(
        //    services.BuildServiceProvider(),
        //    new ApplicationLifetime(new NullLogger<ApplicationLifetime>()));

        TimeToQuery = DateTimeOffset.UtcNow;
        MinPriceAllowed = 0.07M;
        Prices = [.. Enumerable.Range(0, 24)
            .Select(i => new Libs.Core.Entities.Pvpc(
                TimeToQuery.UtcDateTime.AddHours(i),
                decimal.Divide(Random.Shared.Next(40_000, 220_000), 1_000M)))];
        Prices.Last(x => x.AtDateTimeOffset <= TimeToQuery).MWhPriceInEuros = 49M; // 0.049 KWhPriceInEuros
    }

    [Fact]
    public void IsTimeToChargeNoPricesTest()
    {
        Settings.TuyaManagerSettings tuyaManagerSettings = new()
        {
            CronExpression = Cronos.CronExpression.Hourly.ToString(),
            AllowChargeWhenKWhPriceInEurosIsBelowThan = decimal.MaxValue,
            ChargingHoursPerDay = 4,
        };

        bool res = Lib.Services.PvpcCronBackgroundService.IsTimeToCharge(
            [],
            TimeToQuery,
            tuyaManagerSettings,
            new NullLogger<PvpcCronBackgroundServiceTests>());

        Assert.False(res);
    }

    [Fact]
    public void IsTimeToChargeAllowBelowDecimalMaxValueTest()
    {
        Settings.TuyaManagerSettings tuyaManagerSettings = new()
        {
            CronExpression = Cronos.CronExpression.Hourly.ToString(),
            AllowChargeWhenKWhPriceInEurosIsBelowThan = decimal.MaxValue,
            ChargingHoursPerDay = 4,
        };

        bool res = Lib.Services.PvpcCronBackgroundService.IsTimeToCharge(
            Prices,
            TimeToQuery,
            tuyaManagerSettings,
            new NullLogger<PvpcCronBackgroundServiceTests>());

        Assert.True(res);
    }

    [Fact]
    public void IsTimeToChargeIfAnyPriceBelowTest()
    {
        Settings.TuyaManagerSettings tuyaManagerSettings = new()
        {
            CronExpression = Cronos.CronExpression.Hourly.ToString(),
            AllowChargeWhenKWhPriceInEurosIsBelowThan = MinPriceAllowed,
            ChargingHoursPerDay = 4,
        };
        bool res = Lib.Services.PvpcCronBackgroundService.IsTimeToCharge(
            Prices,
            TimeToQuery,
            tuyaManagerSettings,
            new NullLogger<PvpcCronBackgroundServiceTests>());

        Assert.Equal(res, Prices.Any(x => x.KWhPriceInEuros <= MinPriceAllowed));
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
                // TODO: dispose managed state (managed objects)
                PvpcService?.Dispose();

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~PvpcCronBackgroundServiceTests()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
