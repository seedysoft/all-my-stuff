using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seedysoft.Libs.Infrastructure.Extensions;
using System.Threading.Tasks;
using TUnit.Core;

namespace Seedysoft.Libs.Update.Tests.Services;

public sealed class UpdaterCronBackgroundServiceTests : Infrastructure.Tests.TestClassBase
{
    private readonly Update.Services.UpdaterCronBackgroundService updaterCronBackgroundService = default!;

    public UpdaterCronBackgroundServiceTests()
    {
        HostApplicationBuilder appBuilder = new();
        _ = appBuilder.AddAllMyDependencies();
        ServiceProvider serviceProvider = appBuilder.Services.BuildServiceProvider();

        updaterCronBackgroundService = serviceProvider.GetRequiredService<Update.Services.UpdaterCronBackgroundService>();
    }

    [Test]
    public async Task GetLatestReleaseFromGithubAsyncTest()
    {
        Octokit.Release? release = await updaterCronBackgroundService.GetLatestReleaseFromGithubAsync();
        await TUnit.Assertions.Assert.That(release).IsNotNull();

        var RelaseVersion = new Version(release.Name);
        await TUnit.Assertions.Assert.That(RelaseVersion < new Version(DateTime.UtcNow.ToString("yy.Mdd.Hmm.ss"))).IsTrue();
    }

    //[Fact]
    //public async Task DownloadLatestReleaseAssetTest()
    //{
    //    Enums.UpdateResults updateResult = await updaterCronBackgroundService.DownloadLatestReleaseAsset(CancellationToken.None);
    //    Assert.Equal(Enums.UpdateResults.Ok, updateResult);
    //}
}
