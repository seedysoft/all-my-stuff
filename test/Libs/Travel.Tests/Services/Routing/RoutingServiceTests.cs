using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seedysoft.Libs.Infrastructure.Extensions;

namespace Seedysoft.Libs.Travel.Tests.Services.Routing;

/// <summary>
/// Integration tests for the Routing Service.
/// These tests require external service availability and network connectivity.
/// They are marked as [Explicit] to run only when explicitly requested.
/// </summary>
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

    /// <summary>
    /// Integration test: Verifies routing service can fetch routes from external Valhalla API.
    /// Requires: Network connectivity to https://valhalla1.openstreetmap.de
    /// </summary>
    [Test]
    [Explicit("Requires external service availability and network connectivity")]
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
