using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seedysoft.Libs.Infrastructure.Extensions;
using Xunit;

namespace Seedysoft.Libs.GasStationPrices.Tests;

public sealed class ObtainGasStationPricesServiceTests : Infrastructure.Tests.TestClassBase
{
    private readonly Services.ObtainGasStationPricesService ObtainGasStationPricesService = default!;

    public ObtainGasStationPricesServiceTests() : base()
    {
        HostApplicationBuilder appBuilder = new();
        _ = appBuilder.AddAllMyDependencies();
        ServiceProvider serviceProvider = appBuilder.Services.BuildServiceProvider();

        ObtainGasStationPricesService = serviceProvider.GetRequiredService<Services.ObtainGasStationPricesService>();
    }

    //[Fact]
    //public async Task GetPetroleumProductsAsyncTest()
    //{
    //    IEnumerable<Core.Json.Minetur.ProductoPetrolifero> Res =
    //        await ObtainGasStationPricesService.GetPetroleumProductsAsync(CancellationToken.None);

    //    Assert.True(Res.Any());
    //}

    [Fact]
    public async Task GetGasStationsAsyncTest()
    {
        ViewModels.TravelQueryModel travelQueryModel = new()
        {
            Origin = "Juan Ramón Jiménez 8 Burgos",
            Destination = "Calle la Iglesia 11 Brazuelo León",
            MaxDistanceInKm = 50,
            PetroleumProductsSelectedIds = [],
        };

        IAsyncEnumerator<ViewModels.GasStationModel> gasStationModels =
            ObtainGasStationPricesService.GetGasStationsAsync(travelQueryModel, CancellationToken.None).GetAsyncEnumerator();

        _ = await gasStationModels.MoveNextAsync();

        Assert.NotNull(gasStationModels.Current);
        Assert.NotNull(gasStationModels.Current?.Rotulo);
        Assert.False(string.IsNullOrWhiteSpace(gasStationModels.Current?.Rotulo));
    }
}
