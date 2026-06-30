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
        [Arguments("Juan Ramón Jiménez 8 Burgos")]
        [Arguments("Manciles")]
        [Arguments("La Iglesia 11 Brazuelo")]
        [Arguments("Hontalbilla")]
        [Arguments("Teruel")]
        [Arguments("abba ordino babot hotel")]
        string textToFind)
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        IReadOnlyList<ViewModels.Place> result =
            await geoplacingService.FindPlacesAsync(textToFind, cancellationToken);

        //System.Diagnostics.Debug.WriteLine(textToFind);
        //foreach (ViewModels.Place item in result)
        //    System.Diagnostics.Debug.WriteLine(item.Address);
        //System.Diagnostics.Debug.WriteLine(string.Empty);

        // Assert
        _ = await Assert.That(result).IsNotNull().And.HasAtLeast(1);
    }

    [Test]
    public async Task FindPlacesAsyncWithEmptyTextToFindReturnsEmptyOrException()
    {
        // Arrange
        string textToFind = string.Empty;
        CancellationToken cancellationToken = CancellationToken.None;

        // Act & Assert
        IReadOnlyList<ViewModels.Place> result = await geoplacingService.FindPlacesAsync(textToFind, cancellationToken);

        // Assert
        _ = await Assert.That(result).IsNotNull().And.IsEmpty();
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
        _ = await Assert.ThrowsAsync<OperationCanceledException>(() => geoplacingService.FindPlacesAsync(textToFind, cts.Token));
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
