using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Seedysoft.Pvpc.Lib.Tests;

[TestClass]
public sealed class TuyaManagerBackgroundServiceCronTests : Libs.Infrastructure.Tests.TestClassBase
{
    private static Services.TuyaManagerBackgroundServiceCron TuyaManagerService = default!;

    [ClassInitialize(InheritanceBehavior.None)]
    public static void TuyaManagerBackgroundServiceCronTestsClassInitialize(TestContext testContext)
    {
        ClassInitialize(testContext);

        Settings.TuyaManagerSettings tuyaManagerSettings = new()
        {
            CronExpression = "* * 30 2 *", // At every minute on day-of-month 30 in February = never.
        };

        IServiceCollection services = new ServiceCollection();
        _ = services
            .AddSingleton(tuyaManagerSettings)
            .AddSingleton(GetDbCxt())
            .AddSingleton<Microsoft.Extensions.Logging.ILogger<Services.TuyaManagerBackgroundServiceCron>>(new NullLogger<Services.TuyaManagerBackgroundServiceCron>());

        TuyaManagerService = new(
            services.BuildServiceProvider(),
            new ApplicationLifetime(new NullLogger<ApplicationLifetime>()));
    }

    [ClassCleanup(InheritanceBehavior.None, ClassCleanupBehavior.EndOfClass)]
    public static void TuyaManagerBackgroundServiceCronTestsClassCleanup()
    {
        ClassCleanup();
        TuyaManagerService?.Dispose();
    }

    [TestMethod]
    public async Task DoWorkAsyncTest() => await TuyaManagerService.DoWorkAsync(default);
}
