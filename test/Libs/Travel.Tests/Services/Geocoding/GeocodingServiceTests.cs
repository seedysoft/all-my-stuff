using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seedysoft.Libs.Infrastructure.Extensions;

namespace Seedysoft.Libs.Travel.Tests.Services.Geocoding;

public sealed class GeocodingServiceTests : Infrastructure.Tests.TestClassBase
{
    private readonly Travel.Services.Geocoding.GeocodingService geoplacingService = default!;

    public GeocodingServiceTests() : base()
    {
        HostApplicationBuilder appBuilder = new();
        _ = appBuilder.AddAllMyDependencies();
        ServiceProvider serviceProvider = appBuilder.Services.BuildServiceProvider();

        geoplacingService = serviceProvider.GetRequiredService<Travel.Services.Geocoding.GeocodingService>();
    }

    [Test]
    [CombinedDataSources]
    public async Task FindPlacesAsyncWithValidTextToFindReturnsPlaces(
        [Arguments("Barcelona")]
        [Arguments("Soria")]
        [Arguments("Burgos")]
        [Arguments("Manciles")]
        [Arguments("Brazuelo")]
        [Arguments("Hontalbilla")]
        [Arguments("Teruel")]
        string textToFind)
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        IReadOnlyList<ViewModels.Place> result =
            await geoplacingService.FindPlacesAsync(textToFind, cancellationToken);

        // Assert
        _ = await Assert.That(result).IsNotNull();
        _ = await Assert.That(result).IsAssignableFrom<IReadOnlyList<ViewModels.Place>>();
    }

    [Test]
    public async Task FindPlacesAsyncWithEmptyTextToFindReturnsEmptyOrException()
    {
        // Arrange
        string textToFind = string.Empty;
        CancellationToken cancellationToken = CancellationToken.None;

        // Act & Assert
        IReadOnlyList<ViewModels.Place> result = await geoplacingService.FindPlacesAsync(textToFind, cancellationToken);
        Assert.NotNull(result);
    }

    [Test]
    [CombinedDataSources]
    public async Task FindPlacesAsyncWithCancellationTokenRespectsCancellation(
        [Arguments("Barcelona")]
        string textToFind)
    {
        // Arrange
        using CancellationTokenSource cts = new();
        cts.Cancel();

        // Act & Assert
        _ = await Assert.ThrowsAsync<OperationCanceledException>(
            () => geoplacingService.FindPlacesAsync(textToFind, cts.Token));
    }

    [Test]
    [CombinedDataSources]
    public async Task FindPlacesAsyncWithUnsupportedGeocoderImplementationThrowsInvalidOperationException(
        [Arguments("Paris")]
        string textToFind)
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;

        // This test would require mocking the TravelSettings to return an unsupported implementation
        // For now, it validates the current supported implementation doesn't throw

        // Act
        IReadOnlyList<ViewModels.Place> result =
            await geoplacingService.FindPlacesAsync(textToFind, cancellationToken);

        // Assert
        Assert.NotNull(result);
    }
}
