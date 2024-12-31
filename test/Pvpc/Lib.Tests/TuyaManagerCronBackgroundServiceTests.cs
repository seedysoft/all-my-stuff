using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Seedysoft.Pvpc.Lib.Tests;

public sealed class TuyaManagerCronBackgroundServiceTests : Libs.Infrastructure.Tests.TestClassBase
{
    private readonly Services.TuyaManagerCronBackgroundService TuyaManagerService = default!;

    public TuyaManagerCronBackgroundServiceTests() : base()
    {
        Settings.TuyaManagerSettings tuyaManagerSettings = new()
        {
            CronExpression = "* * 30 2 *", // At every minute on day-of-month 30 in February.
        };

        IServiceCollection services = new ServiceCollection();
        _ = services
            .AddSingleton(tuyaManagerSettings)
            .AddSingleton(GetDbCxt())
            .AddSingleton<Microsoft.Extensions.Logging.ILogger<Services.TuyaManagerCronBackgroundService>>(new NullLogger<Services.TuyaManagerCronBackgroundService>());

        TuyaManagerService = new(
            services.BuildServiceProvider(),
            new ApplicationLifetime(new NullLogger<ApplicationLifetime>()));
    }

    [Fact]
    public async Task DoWorkAsyncTest() => await TuyaManagerService.DoWorkAsync(default);

    protected override void Dispose(bool disposing)
    {
        TuyaManagerService?.Dispose();
        Dispose();
    }
}
