using Microsoft.Extensions.DependencyInjection;
using Seedysoft.Libs.Infrastructure.Extensions;
using Xunit;

namespace Seedysoft.Libs.Rpt.Tests;

public sealed class RptCronBackgroundServiceTests : Infrastructure.Tests.TestClassBase
{
    private readonly Services.RptCronBackgroundService rptCronBackgroundService;

    public RptCronBackgroundServiceTests(Xunit.Abstractions.ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        Microsoft.Extensions.Hosting.HostApplicationBuilder appBuilder = new();
        _ = appBuilder.AddAllMyDependencies();
        ServiceProvider serviceProvider = appBuilder.Services.BuildServiceProvider();

        rptCronBackgroundService = serviceProvider.GetRequiredService<Services.RptCronBackgroundService>();
    }

    [Fact(Timeout = 3_600_000)]
    public async Task DoWorkAsyncTest() => await rptCronBackgroundService.DoWorkAsync(CancellationToken.None);
}
