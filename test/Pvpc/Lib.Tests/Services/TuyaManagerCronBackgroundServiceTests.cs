using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Seedysoft.Pvpc.Lib.Tests.Services;

public sealed class TuyaManagerCronBackgroundServiceTests : Libs.Infrastructure.Tests.TestClassBase, IDisposable
{
    private readonly Lib.Services.TuyaManagerCronBackgroundService TuyaManagerService = default!;
    private bool disposedValue;

    public TuyaManagerCronBackgroundServiceTests(Xunit.Abstractions.ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.{nameof(Settings.TuyaManagerSettings)}.Tests.json", optional: false, reloadOnChange: true)
            .Build();

        IServiceCollection services = new ServiceCollection();
        _ = services
            .AddSingleton(configuration)
            .AddSingleton<Microsoft.Extensions.Logging.ILogger<Lib.Services.TuyaManagerCronBackgroundService>>(new NullLogger<Lib.Services.TuyaManagerCronBackgroundService>());

        AddDbContext(services);

        TuyaManagerService = new(
            services.BuildServiceProvider(),
            new ApplicationLifetime(new NullLogger<ApplicationLifetime>()));
    }

    [Fact]
    public async Task DoWorkAsyncTest()
    {
        await TuyaManagerService.DoWorkAsync(default);

        Assert.True(true);
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
                // dispose managed state (managed objects)
                TuyaManagerService?.Dispose();

            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null
            disposedValue = true;
        }
    }

    // // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~TuyaManagerCronBackgroundServiceTests()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
