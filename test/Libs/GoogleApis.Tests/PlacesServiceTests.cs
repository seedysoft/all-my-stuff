using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seedysoft.Libs.Utils.Extensions;
using Xunit;

namespace Seedysoft.Libs.GoogleApis.Tests;

public sealed class PlacesServiceTests : Infrastructure.Tests.TestClassBase
{
    private readonly Services.PlacesService PlacesService = default!;

    public PlacesServiceTests() : base()
    {
        HostApplicationBuilder appBuilder = new();
        _ = appBuilder.AddAllMyDependencies();
        ServiceProvider serviceProvider = appBuilder.Services.BuildServiceProvider();

        PlacesService = serviceProvider.GetRequiredService<Services.PlacesService>();
    }

    [InlineData("Calle de la Iglesia 11 Brazuelo León")]
    [InlineData("Juan Ramón Jiménez 8 Burgos")]
    [Theory]
    public async Task FindPlacesAsyncTest(string textToFind)
    {
        IEnumerable<string> Res =
            await PlacesService.FindPlacesAsync(textToFind, CancellationToken.None);

        Assert.True(Res.Any());
    }

    protected override void Dispose(bool disposing) => Dispose();
}
