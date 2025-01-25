using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seedysoft.Libs.Infrastructure.Extensions;
using Xunit;

namespace Seedysoft.Libs.Update.Tests;

public sealed class UpdaterCronBackgroundServiceTests : Infrastructure.Tests.TestClassBase
{
    private readonly Services.UpdaterCronBackgroundService updaterCronBackgroundService = default!;

    public UpdaterCronBackgroundServiceTests() : base()
    {
        HostApplicationBuilder appBuilder = new();
        _ = appBuilder.AddAllMyDependencies();
        ServiceProvider serviceProvider = appBuilder.Services.BuildServiceProvider();

        updaterCronBackgroundService = serviceProvider.GetRequiredService<Services.UpdaterCronBackgroundService>();
    }

    [Fact]
    public async Task GetLatestVersionFromGithubTest()
    {
        Version? version = await updaterCronBackgroundService.GetLatestVersionFromGithubAsync();

        Assert.NotNull(version);
    }
}
