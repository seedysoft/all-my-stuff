using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seedysoft.Libs.Infrastructure.Extensions;
using Xunit;

namespace Seedysoft.Libs.GoogleApis.Tests.Services;

public sealed class PlacesServiceTests : Infrastructure.Tests.TestClassBase
{
    private readonly GoogleApis.Services.PlacesService PlacesService = default!;

    public PlacesServiceTests(Xunit.Abstractions.ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        HostApplicationBuilder appBuilder = new();
        _ = appBuilder.AddAllMyDependencies();
        ServiceProvider serviceProvider = appBuilder.Services.BuildServiceProvider();

        PlacesService = serviceProvider.GetRequiredService<GoogleApis.Services.PlacesService>();
    }

    [InlineData("Calle de la Iglesia 11 Brazuelo León")]
    [InlineData("Juan Ramón Jiménez 8 Burgos")]
    [InlineData("Azud de Villagonzalo")]
    [Theory]
    public async Task FindPlacesAsyncTest(string textToFind)
    {
        IEnumerable<string> Res =
            await PlacesService.FindPlacesAsync(textToFind, CancellationToken.None);

        Assert.True(Res.Any());
    }
}
