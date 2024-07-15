using Seedysoft.Libs.Utils.Dependencies;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seedysoft.Update.Lib.Services;

namespace Seedysoft.Update.Lib.Tests.Services;

[TestClass]
public sealed class UpdateBackgroundServiceCronTest : Libs.Infrastructure.Tests.TestClassBase
{
    private static UpdateBackgroundServiceCron UpdateService = default!;

    [TestInitialize]
    public void TestInitialize()
    {

    }

    [ClassInitialize]
    public static new void ClassInitialize(TestContext testContext)
    {
        Microsoft.Extensions.Hosting.HostApplicationBuilder hostApplicationBuilder = new();

        new Dependencies.Configurator().AddDependencies(hostApplicationBuilder);

        hostApplicationBuilder.Services.TryAddSingleton<Microsoft.Extensions.Logging.ILogger<UpdateBackgroundServiceCron>>(new NullLogger<UpdateBackgroundServiceCron>());
        hostApplicationBuilder.Services.TryAddSingleton<Microsoft.Extensions.Hosting.IHostApplicationLifetime>(new ApplicationLifetime(new NullLogger<ApplicationLifetime>()));

        Microsoft.Extensions.Hosting.IHost host = hostApplicationBuilder.Build();

        UpdateService = new(host.Services);
    }

    [ClassCleanup]
    public static new void ClassCleanup() => UpdateService?.Dispose();

    [TestMethod]
    public async Task IsConnectionTest() => Assert.IsNotNull(await UpdateService.ConnectAsync());

    [TestMethod]
    public async Task DoWorkAsyncTest() => await UpdateService.DoWorkAsync(CancellationToken.None);

    [TestMethod]
    public void ExtractFileTest()
    {
        const string extractorFileName = @"C:\Program Files\7-Zip\7z.exe";
        const string tempDir = @"C:\Users\alfon\AppData\Local\Temp\Update-Release v1.1.1.6-638565582644439810";
        string sourceFileName = Path.Combine(tempDir, "win-x64.v1.1.1.6.7z");

        if (File.Exists(sourceFileName))
            UpdateService.ExtractFile(extractorFileName, sourceFileName, tempDir);
        else
            Assert.Inconclusive();
    }
}