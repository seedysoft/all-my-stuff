#pragma warning disable CA1873 // Avoid potentially expensive logging

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Seedysoft.Photo.Lib.Tests.Services;

public sealed class PhotoCronBackgroundServiceTests : Libs.Core.Tests.TestClassBase, IDisposable
{
    private Mock<ILogger<Lib.Services.PhotoCronBackgroundService>> MockLogger = null!;
    private Mock<DbContexts.PhotoCtx> MockPhotoCtx = null!;

    private Lib.Services.PhotoCronBackgroundService photoCronBackgroundService = default!;

    //[Before(Test)]
    // TUnit0074
    // When a virtual instance hook method ([Before(Test)] / [After(Test)]) is already declared on a base class, applying the same hook attribute to an override causes the hook to be registered twice.
    // Because virtual dispatch routes both registrations to the same override, the override's body runs twice per test. Remove the attribute from the override — the base class hook will still call through to the override via virtual dispatch.
    public override void SetupTest()
    {
        base.SetupTest();

        MockLogger = new Mock<ILogger<Lib.Services.PhotoCronBackgroundService>>();
        MockPhotoCtx = new Mock<DbContexts.PhotoCtx>();

        SetupDefaultMocks();

        photoCronBackgroundService = new Lib.Services.PhotoCronBackgroundService(MockServiceProvider.Object, MockHostApplicationLifetime.Object);
    }

    //[After(Test)]
    // TUnit0074
    // When a virtual instance hook method ([Before(Test)] / [After(Test)]) is already declared on a base class, applying the same hook attribute to an override causes the hook to be registered twice.
    // Because virtual dispatch routes both registrations to the same override, the override's body runs twice per test. Remove the attribute from the override — the base class hook will still call through to the override via virtual dispatch.
    public override void CleanupTest()
    {
        MockPhotoCtx.Object.Dispose();
        photoCronBackgroundService?.Dispose();

        base.CleanupTest();
    }

    private void SetupDefaultMocks()
    {
        // Setup logger as enabled for all levels
        _ = MockLogger
            .Setup(static x => x.IsEnabled(It.IsAny<LogLevel>()))
            .Returns(true);

        // Create a real configuration builder with the test data
        ConfigurationBuilder configurationBuilder = new();
        _ = configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
        {
            { $"{nameof(Settings.PhotoSettings)}:CronExpression", "* * 30 2 *" },
            { $"{nameof(Settings.PhotoSettings)}:DelayBetweenExecutionsTimeSpan", "00:00:01" },
            { $"{nameof(Settings.PhotoSettings)}:TimeZoneInfo", TimeZoneInfo.Local.Id },
            { $"{nameof(Settings.PhotoSettings)}:PhotoRootPath", Path.Join(Environment.GetEnvironmentVariable("USERPROFILE"), @"\od\_Syncthing\Photos") },
        });

        IConfigurationRoot configurationRoot = configurationBuilder.Build();
        IConfigurationSection configurationSection = configurationRoot.GetSection(nameof(Settings.PhotoSettings));

        // Setup configuration section to return real section
        _ = MockConfiguration
            .Setup(static x => x.GetSection(nameof(Settings.PhotoSettings)))
            .Returns(configurationSection);

        // Setup service provider to return required services
        _ = MockServiceProvider
            .Setup(static x => x.GetService(typeof(ILogger<Lib.Services.PhotoCronBackgroundService>)))
            .Returns(MockLogger.Object);

        _ = MockServiceProvider
            .Setup(static x => x.GetService(typeof(IConfiguration)))
            .Returns(MockConfiguration.Object);

        _ = MockServiceProvider
            .Setup(static x => x.GetService(typeof(DbContexts.PhotoCtx)))
            .Returns(MockPhotoCtx.Object);
    }

    [Test]
    public async Task DoWorkAsyncShouldGetPhotoCtxFromPhotoCronBackgroundServiceProvider()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationTokenSource.Token;

        // Act
        await photoCronBackgroundService.DoWorkAsync(cancellationToken);

        // Assert
        MockServiceProvider.Verify(
            expression: static x => x.GetService(typeof(DbContexts.PhotoCtx)),
            times: Times.Once);
    }

    [Test]
    public async Task DoWorkAsyncShouldCompleteSuccessfully()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationTokenSource.Token;

        // Act & Assert (Should not throw)
        await photoCronBackgroundService.DoWorkAsync(cancellationToken);
    }

    [Test]
    public async Task DoWorkAsyncShouldHandleExceptionGracefully()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationTokenSource.Token;

        _ = MockServiceProvider
            .Setup(static x => x.GetService(typeof(DbContexts.PhotoCtx)))
            .Throws(new InvalidOperationException("Service not found"));

        // Act & Assert (Should not throw)
        await photoCronBackgroundService.DoWorkAsync(cancellationToken);

        // Verify exception was logged
        MockLogger.Verify(
            static x => x.Log(
                logLevel: It.IsAny<LogLevel>(),
                eventId: It.IsAny<EventId>(),
                state: It.IsAny<It.IsAnyType>(),
                exception: It.IsAny<Exception>(),
                formatter: It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Test]
    public async Task DoWorkAsyncWithCancellationTokenShouldRespectCancellation()
    {
        // Arrange
        CancellationTokenSource.Cancel();
        CancellationToken cancellationToken = CancellationTokenSource.Token;

        // Act & Assert (Should not throw even with cancelled token)
        await photoCronBackgroundService.DoWorkAsync(cancellationToken);
    }

    [Test]
    [CombinedDataSources]
    public async Task TryToObtainPhotoMetadataTest(
        [Arguments(@"C:\Users\amt\od\_Syncthing\Fotos\IMG_20261109_144720.jpg") ]
        string photoFullPath)
    {
        if (File.Exists(photoFullPath))
        {
            FileInfo photoFile = new(photoFullPath);
            Entities.Photo photoDb = new() { FilePath = Path.GetRelativePath(photoCronBackgroundService!.Settings.PhotoRootPath, photoFile.FullName) };

            _ = photoCronBackgroundService.TryToObtainPhotoMetadata(photoFile, photoDb);
        }
    }

    public void Dispose() => photoCronBackgroundService?.Dispose();
}
