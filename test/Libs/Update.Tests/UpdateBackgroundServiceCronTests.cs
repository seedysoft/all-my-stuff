using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Seedysoft.Libs.Update.Tests;

[TestClass]
public sealed class UpdateBackgroundServiceCronTest : Infrastructure.Tests.TestClassBase
{
    private static Services.UpdateBackgroundServiceCron UpdateService = default!;

    [ClassInitialize]
    public static new void ClassInitialize(TestContext testContext)
    {
        Microsoft.Extensions.Hosting.HostApplicationBuilder hostApplicationBuilder =
            Microsoft.Extensions.Hosting.Host.CreateEmptyApplicationBuilder(null);

        new Dependencies.Configurator().AddDependencies(hostApplicationBuilder);

        hostApplicationBuilder.Services.TryAddSingleton(GetDbCxt());
        hostApplicationBuilder.Services.TryAddSingleton<Microsoft.Extensions.Logging.ILogger<Services.UpdateBackgroundServiceCron>>(new NullLogger<Services.UpdateBackgroundServiceCron>());
        hostApplicationBuilder.Services.TryAddSingleton<Microsoft.Extensions.Hosting.IHostApplicationLifetime>(new ApplicationLifetime(new NullLogger<ApplicationLifetime>()));

        Microsoft.Extensions.Hosting.IHost host = hostApplicationBuilder.Build();

        UpdateService = new(host.Services);
    }

    [ClassCleanup]
    public static new void ClassCleanup() => UpdateService?.Dispose();

    [TestMethod]
    public async Task IsConnectionTest() => Assert.IsTrue(await UpdateService.ConnectAsync());
}
