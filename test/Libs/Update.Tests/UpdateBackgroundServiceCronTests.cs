using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Seedysoft.Libs.Update.Test;

public sealed class UpdateBackgroundServiceCronTest(UpdateBackgroundServiceCronFixture updateServiceFixture)
    : IClassFixture<UpdateBackgroundServiceCronFixture>
{
    public UpdateBackgroundServiceCronFixture UpdateServiceFixture { get; } = updateServiceFixture;

    [Fact]
    public async Task IsConnectionTest() => Assert.True(await UpdateServiceFixture.UpdateService.ConnectAsync());

    //[Fact]
    //public void IsTimeToChargeAllowBelowDecimalMaxValue()
    //{
    //    Settings.TuyaManagerSettings tuyaManagerSettings = new()
    //    {
    //        CronExpression = Cronos.CronExpression.Hourly.ToString(),
    //        AllowChargeWhenKWhPriceInEurosIsBelowThan = decimal.MaxValue,
    //        ChargingHoursPerDay = 4,
    //    };

    //    bool res = Services.UpdateBackgroundServiceCron.IsTimeToCharge(
    //        UpdateServiceFixture.Prices,
    //        UpdateServiceFixture.TimeToQuery,
    //        tuyaManagerSettings);

    //    Assert.True(res);
    //}

    //[Fact]
    //public void IsTimeToChargeIfAnyPriceBelow()
    //{
    //    Settings.TuyaManagerSettings tuyaManagerSettings = new()
    //    {
    //        CronExpression = Cronos.CronExpression.Hourly.ToString(),
    //        AllowChargeWhenKWhPriceInEurosIsBelowThan = UpdateServiceFixture.MinPriceAllowed,
    //        ChargingHoursPerDay = 4,
    //    };
    //    bool res = Services.UpdateBackgroundServiceCron.IsTimeToCharge(
    //        UpdateServiceFixture.Prices,
    //        UpdateServiceFixture.TimeToQuery,
    //        tuyaManagerSettings);

    //    Assert.Equal(res, UpdateServiceFixture.Prices.Any(x => x.KWhPriceInEuros <= UpdateServiceFixture.MinPriceAllowed));
    //}
}

public sealed class UpdateBackgroundServiceCronFixture : Infrastructure.Test.BaseFixture
{
    public UpdateBackgroundServiceCronFixture() : base()
    {
        Microsoft.Extensions.Hosting.HostApplicationBuilder hostApplicationBuilder =
            Microsoft.Extensions.Hosting.Host.CreateEmptyApplicationBuilder(null);

        new Dependencies.Configurator().AddDependencies(hostApplicationBuilder);

        hostApplicationBuilder.Services.TryAddSingleton(GetDbCxt());
        hostApplicationBuilder.Services.TryAddSingleton<Microsoft.Extensions.Logging.ILogger<Services.UpdateBackgroundServiceCron>>(new NullLogger<Services.UpdateBackgroundServiceCron>());
        hostApplicationBuilder.Services.TryAddSingleton<Microsoft.Extensions.Hosting.IHostApplicationLifetime>(new ApplicationLifetime(new NullLogger<ApplicationLifetime>()));

        Microsoft.Extensions.Hosting.IHost host = hostApplicationBuilder.Build();

        UpdateService = new(host.Services);
    }

    public Services.UpdateBackgroundServiceCron UpdateService { get; }

    public override void Dispose()
    {
        UpdateService?.Dispose();
        base.Dispose();
    }
}
