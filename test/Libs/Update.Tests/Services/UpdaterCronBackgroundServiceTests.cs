using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seedysoft.Libs.Infrastructure.Extensions;
using System.Threading.Tasks;

namespace Seedysoft.Libs.Update.Tests.Services;

public sealed class UpdaterCronBackgroundServiceTests : Infrastructure.Tests.TestClassBase
{
    private readonly Update.Services.UpdaterCronBackgroundService updaterCronBackgroundService = default!;

    public UpdaterCronBackgroundServiceTests() : base(testOutputHelper)
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
        Assert.NotNull(release);

        var RelaseVersion = new Version(release.Name);
        await Assert.That(RelaseVersion < new Version(DateTime.UtcNow.ToString("yy.Mdd.Hmm.ss"))).IsTrue();
    }

    //[Fact]
    //public async Task DownloadLatestReleaseAssetTest()
    //{
    //    Enums.UpdateResults updateResult = await updaterCronBackgroundService.DownloadLatestReleaseAsset(CancellationToken.None);
    //    Assert.Equal(Enums.UpdateResults.Ok, updateResult);
    //}
}