using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seedysoft.Libs.Utils.Extensions;

namespace Seedysoft.Libs.GasStationPrices.Tests;

[TestClass]
public sealed class ObtainGasStationPricesTests : Infrastructure.Tests.TestClassBase
{
    private static Services.ObtainGasStationPricesService ObtainGasStationPricesService = default!;

    [ClassInitialize(InheritanceBehavior.None)]
    public static void ClassInitialize(TestContext testContext)
    {
        IHostApplicationBuilder appBuilder = new HostApplicationBuilder();
        _ = appBuilder.AddAllMyDependencies();
        ServiceProvider serviceProvider = appBuilder.Services.BuildServiceProvider();

        ObtainGasStationPricesService = serviceProvider.GetRequiredService<Services.ObtainGasStationPricesService>();
    }
    //[ClassCleanup(InheritanceBehavior.None, ClassCleanupBehavior.EndOfClass)]
    //public static new void ClassCleanup() => ObtainGasStationPricesService?.Dispose();

    [TestMethod]
    public async Task GetPetroleumProductsAsyncTest()
    {
        IEnumerable<Core.Json.Minetur.ProductoPetrolifero> Res =
            await ObtainGasStationPricesService.GetPetroleumProductsAsync(CancellationToken.None);

        Assert.IsTrue(Res.Any());
    }

    [TestMethod]
    [DataRow("Calle de la Iglesia 11 Brazuelo León")]
    [DataRow("Juan Ramón Jiménez 8 Burgos")]
    public async Task FindPlacesAsyncTest(string textToFind)
    {
        IEnumerable<string> Res =
            await ObtainGasStationPricesService.FindPlacesAsync(textToFind, CancellationToken.None);

        Assert.IsTrue(Res.Any());
    }

    [TestMethod]
    public async Task GetGasStationsAsyncTest()
    {
        Core.ViewModels.TravelQueryModel travelQueryModel = new()
        {
            Origin = "Juan Ramón Jiménez 8 Burgos",
            Destination = "Calle la Iglesia 11 Brazuelo León",
            MaxDistanceInKm = 50,
            PetroleumProductsSelectedIds = [],
        };

        IAsyncEnumerator<Core.ViewModels.GasStationModel> gasStationModels =
            ObtainGasStationPricesService.GetGasStationsAsync(travelQueryModel, CancellationToken.None).GetAsyncEnumerator();

        _ = await gasStationModels.MoveNextAsync();

        Assert.IsTrue(gasStationModels.Current != null && !string.IsNullOrWhiteSpace(gasStationModels.Current?.Rotulo));
    }
}
