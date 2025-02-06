using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Seedysoft.Libs.Infrastructure.Extensions;
using Xunit;

namespace Seedysoft.Pvpc.Lib.Tests;

public sealed class TuyaManagerCronBackgroundServiceTests : Libs.Infrastructure.Tests.TestClassBase, IDisposable
{
    private readonly Services.TuyaManagerCronBackgroundService TuyaManagerService = default!;
    private bool disposedValue;

    public TuyaManagerCronBackgroundServiceTests() : base()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.{nameof(Settings.TuyaManagerSettings)}.Tests.json", optional: false, reloadOnChange: true)
            .Build();

        IServiceCollection services = new ServiceCollection();
        _ = services
            .AddSingleton(configuration)
            .AddSingleton<Microsoft.Extensions.Logging.ILogger<Services.TuyaManagerCronBackgroundService>>(new NullLogger<Services.TuyaManagerCronBackgroundService>());

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
            {
                // TODO: dispose managed state (managed objects)
                TuyaManagerService?.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
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
