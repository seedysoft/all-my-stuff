using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Seedysoft.Libs.Update.Tests.Services;

[TestClass]
public sealed class UpdateBackgroundServiceCronTest : Infrastructure.Tests.TestClassBase
{
    private static Update.Services.UpdateBackgroundServiceCron UpdateService = default!;

    [TestInitialize]
    public void TestInitialize()
    {

    }

    [ClassInitialize]
    public static new void ClassInitialize(TestContext testContext)
    {
        Microsoft.Extensions.Hosting.HostApplicationBuilder hostApplicationBuilder = new();

        new Dependencies.Configurator().AddDependencies(hostApplicationBuilder);

        hostApplicationBuilder.Services.TryAddSingleton<Microsoft.Extensions.Logging.ILogger<Update.Services.UpdateBackgroundServiceCron>>(new NullLogger<Update.Services.UpdateBackgroundServiceCron>());
        hostApplicationBuilder.Services.TryAddSingleton<Microsoft.Extensions.Hosting.IHostApplicationLifetime>(new ApplicationLifetime(new NullLogger<ApplicationLifetime>()));

        Microsoft.Extensions.Hosting.IHost host = hostApplicationBuilder.Build();

        UpdateService = new(host.Services);
    }

    [ClassCleanup]
    public static new void ClassCleanup() => UpdateService?.Dispose();

    [TestMethod]
    public async Task IsConnectionTest() => Assert.IsTrue(await UpdateService.ConnectAsync());

    [TestMethod]
    public async Task DoWorkAsyncTest() => await UpdateService.DoWorkAsync(CancellationToken.None);
}
