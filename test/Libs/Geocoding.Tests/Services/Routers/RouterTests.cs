using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seedysoft.Libs.Infrastructure.Extensions;

namespace Seedysoft.Libs.Geocoding.Tests.Services.Routers;

public sealed class RouterTests : Infrastructure.Tests.TestClassBase
{
    private readonly Geocoding.Services.Routers.RoutesService routesService = default!;

    public RouterTests() : base()
    {
        HostApplicationBuilder appBuilder = new();
        _ = appBuilder.AddAllMyDependencies();
        ServiceProvider serviceProvider = appBuilder.Services.BuildServiceProvider();

        routesService = serviceProvider.GetRequiredService<Geocoding.Services.Routers.RoutesService>();
    }

    [Test]
    public async Task GetLatestReleaseFromGithubAsyncTest()
    {
        var Orig = new Models.Location() { Latitude = Constants.Earth.Burgos.Lat, Longitude = Constants.Earth.Burgos.Lng };
        var Dest = new Models.Location() { Latitude = Constants.Earth.Brazuelo.Lat, Longitude = Constants.Earth.Brazuelo.Lng };

        IList<(string NombreRuta, double[,] Coordenadas)> Result = await routesService.GetRoutesAsync(Orig, Dest, CancellationToken.None);

        _ = await Assert.That(Result).IsNotNull();
        _ = await Assert.That(Result.Any()).IsTrue();
    }
}
