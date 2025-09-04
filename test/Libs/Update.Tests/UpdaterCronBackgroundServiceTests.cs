using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seedysoft.Libs.Infrastructure.Extensions;
using Xunit;

namespace Seedysoft.Libs.Update.Tests;

public sealed class UpdaterCronBackgroundServiceTests : Infrastructure.Tests.TestClassBase
{
    private readonly Services.UpdaterCronBackgroundService updaterCronBackgroundService = default!;

    public UpdaterCronBackgroundServiceTests(Xunit.Abstractions.ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        HostApplicationBuilder appBuilder = new();
        _ = appBuilder.AddAllMyDependencies();
        ServiceProvider serviceProvider = appBuilder.Services.BuildServiceProvider();

        updaterCronBackgroundService = serviceProvider.GetRequiredService<Services.UpdaterCronBackgroundService>();
    }

    [Fact]
    public async Task GetLatestReleaseFromGithubAsyncTest()
    {
        Octokit.Release? release = await updaterCronBackgroundService.GetLatestReleaseFromGithubAsync();
        Assert.NotNull(release);

        var RelaseVersion = new Version(release.Name);
        Assert.True(RelaseVersion < new Version(DateTime.UtcNow.ToString("yy.Mdd.Hmm.ss")));
    }

    [Fact]
    public async Task DownloadLatestReleaseAssetTest()
    {
        Enums.UpdateResults updateResult = await updaterCronBackgroundService.DownloadLatestReleaseAsset(CancellationToken.None);
        Assert.Equal(Enums.UpdateResults.Ok, updateResult);
    }

    [Fact(Timeout = 60_000)]
    public async Task DownloadReleaseFromGithubAsyncTest()
    {
        Octokit.Release? release = await updaterCronBackgroundService.GetLatestReleaseFromGithubAsync();
        Assert.NotNull(release);

        string fileName = await updaterCronBackgroundService.DownloadReleaseFromGithubAsync(release);
        Assert.False(string.IsNullOrWhiteSpace(fileName));

        File.Delete(fileName);
    }
}
