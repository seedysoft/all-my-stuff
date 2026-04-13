using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seedysoft.Libs.Infrastructure.Extensions;
using System.Threading.Tasks;
using TUnit.Core;

namespace Seedysoft.Libs.GoogleApis.Tests.Services;

public sealed class PlacesServiceTests : Infrastructure.Tests.TestClassBase
{
    private readonly GoogleApis.Services.PlacesService PlacesService = default!;

    public PlacesServiceTests()
    {
        HostApplicationBuilder appBuilder = new();
        _ = appBuilder.AddAllMyDependencies();
        ServiceProvider serviceProvider = appBuilder.Services.BuildServiceProvider();

        PlacesService = serviceProvider.GetRequiredService<GoogleApis.Services.PlacesService>();
    }

    [Arguments("Calle de la Iglesia 11 Brazuelo León")]
    [Arguments("Juan Ramón Jiménez 8 Burgos")]
    [Arguments("Azud de Villagonzalo")]
    [Test]
    public async Task FindPlacesAsyncTest(string textToFind)
    {
        IEnumerable<string> Res =
            await PlacesService.FindPlacesAsync(textToFind, CancellationToken.None);

        await TUnit.Assertions.Assert.That(Res.Any()).IsTrue();
    }
}
