using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seedysoft.Libs.Infrastructure.Extensions;

namespace Seedysoft.Libs.Travel.Tests.Services.Routing;

public sealed class RoutingServiceTests : Infrastructure.Tests.TestClassBase
{
    private readonly Travel.Services.Routing.RoutingService routingService = default!;

    public RoutingServiceTests() : base()
    {
        HostApplicationBuilder appBuilder = new();
        _ = appBuilder.AddAllMyDependencies();
        ServiceProvider serviceProvider = appBuilder.Services.BuildServiceProvider();

        routingService = serviceProvider.GetRequiredService<Travel.Services.Routing.RoutingService>();
    }

    [Test]
    public async Task GetRoutesAsyncTest()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        IReadOnlyList<(string NombreRuta, double[,] Coordenadas)> result =
            await routingService.GetRoutesAsync(Constants.Earth.Burgos, Constants.Earth.Brazuelo, cancellationToken);

        // Assert
        _ = await Assert.That(result).IsNotNull();
        _ = await Assert.That(result.Any()).IsTrue();
    }
}
