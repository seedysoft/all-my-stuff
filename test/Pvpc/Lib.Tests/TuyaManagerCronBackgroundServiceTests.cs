using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging.Abstractions;

namespace Seedysoft.Pvpc.Lib.Tests;

[TestClass]
public sealed class TuyaManagerCronBackgroundServiceTests : Libs.Infrastructure.Tests.TestClassBase
{
    private static Services.TuyaManagerCronBackgroundService TuyaManagerService = default!;

    [ClassInitialize(InheritanceBehavior.None)]
    public static void ClassInitialize(TestContext context)
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

    [ClassCleanup(InheritanceBehavior.None, ClassCleanupBehavior.EndOfClass)]
    public static void ClassCleanup() => TuyaManagerService?.Dispose();

    [TestMethod]
    public async Task DoWorkAsyncTest() => await TuyaManagerService.DoWorkAsync(default);
}
