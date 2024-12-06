using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seedysoft.Libs.Utils.Extensions;

namespace Seedysoft.Libs.GoogleApis.Tests;

[TestClass]
public sealed class PlacesServiceTests : Infrastructure.Tests.TestClassBase
{
    private static Services.PlacesService PlacesService = default!;

    [ClassInitialize(InheritanceBehavior.None)]
    public static void ClassInitialize(TestContext testContext)
    {
        HostApplicationBuilder appBuilder = new();
        _ = appBuilder.AddAllMyDependencies();
        ServiceProvider serviceProvider = appBuilder.Services.BuildServiceProvider();

        PlacesService = serviceProvider.GetRequiredService<Services.PlacesService>();
    }

    [DataRow("Calle de la Iglesia 11 Brazuelo León")]
    [DataRow("Juan Ramón Jiménez 8 Burgos")]
    [TestMethod]
    public async Task FindPlacesAsyncTest(string textToFind)
    {
        IEnumerable<string> Res =
            await PlacesService.FindPlacesAsync(textToFind, CancellationToken.None);

        Assert.IsTrue(Res.Any());
    }
}
