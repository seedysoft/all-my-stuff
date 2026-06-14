using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seedysoft.Libs.Infrastructure.Extensions;

namespace Seedysoft.Libs.Travel.Tests.Services.Routing;

public sealed class RoutingTests : Infrastructure.Tests.TestClassBase
{
    private readonly Travel.Services.Routing.RoutingService routingService = default!;

    public RoutingTests() : base()
    {
        HostApplicationBuilder appBuilder = new();
        _ = appBuilder.AddAllMyDependencies();
        ServiceProvider serviceProvider = appBuilder.Services.BuildServiceProvider();

        routingService = serviceProvider.GetRequiredService<Travel.Services.Routing.RoutingService>();
    }

    [Test]
    public async Task GetRoutesAsyncTest()
    {
        var Orig = new Models.Location() { Latitude = Constants.Earth.Burgos.Lat, Longitude = Constants.Earth.Burgos.Lng };
        var Dest = new Models.Location() { Latitude = Constants.Earth.Brazuelo.Lat, Longitude = Constants.Earth.Brazuelo.Lng };

        IList<(string NombreRuta, double[,] Coordenadas)> Result = await routingService.GetRoutesAsync(Orig, Dest, CancellationToken.None);

        _ = await Assert.That(Result).IsNotNull();
        _ = await Assert.That(Result.Any()).IsTrue();
    }
}
