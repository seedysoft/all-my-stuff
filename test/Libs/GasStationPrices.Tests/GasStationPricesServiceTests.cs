using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seedysoft.Libs.Infrastructure.Extensions;
using Xunit;

namespace Seedysoft.Libs.GasStationPrices.Tests;

public sealed class GasStationPricesServiceTests : Infrastructure.Tests.TestClassBase
{
    private readonly Services.GasStationPricesService GasStationPricesService = default!;

    public GasStationPricesServiceTests() : base()
    {
        HostApplicationBuilder appBuilder = new();
        _ = appBuilder.AddAllMyDependencies();
        ServiceProvider serviceProvider = appBuilder.Services.BuildServiceProvider();

        GasStationPricesService = serviceProvider.GetRequiredService<Services.GasStationPricesService>();
    }

    //[Fact]
    //public async Task GetPetroleumProductsAsyncTest()
    //{
    //    IEnumerable<Core.Json.Minetur.ProductoPetrolifero> Res =
    //        await GasStationPricesService.GetPetroleumProductsAsync(CancellationToken.None);

    //    Assert.True(Res.Any());
    //}

    [Fact]
    public async Task GetNearGasStationsAsyncTest()
    {
        //ViewModels.TravelQueryModel travelQueryModel = new()
        //{
        //    Origin = "Juan Ramón Jiménez 8 Burgos",
        //    Destination = "Calle la Iglesia 11 Brazuelo León",
        //    MaxDistanceInKm = 50,
        //    PetroleumProductsSelectedIds = [],
        //};

        //IEnumerable<ViewModels.GasStationModel> gasStationModels = await
        //    GasStationPricesService.GetNearGasStationsAsync(travelQueryModel);

        //Assert.True(gasStationModels.Any()/*.Current != null && !string.IsNullOrWhiteSpace(gasStationModels.Current?.Rotulo)*/);
        Assert.Fail("Must be fixed");
    }

    protected override void Dispose(bool disposing) => Dispose();
}
