using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seedysoft.Libs.Infrastructure.Extensions;

namespace Seedysoft.Libs.GasStationPrices.Tests.Services.Routers;

public sealed class RouterTests : Infrastructure.Tests.TestClassBase
{
    private readonly GasStationPrices.Services.RoutesService routesService = default!;

    public RouterTests() : base()
    {
        HostApplicationBuilder appBuilder = new();
        _ = appBuilder.AddAllMyDependencies();
        ServiceProvider serviceProvider = appBuilder.Services.BuildServiceProvider();

        routesService = serviceProvider.GetRequiredService<GasStationPrices.Services.RoutesService>();
    }

    [Test]
    public async Task GetLatestReleaseFromGithubAsyncTest()
    {
        var model = ViewModels.TravelQueryModel.CreateDefault();

        //IList<(string NombreRuta, double[][] Coordenadas)> Result = await routesService.GetRoutesAsync(model, CancellationToken.None);
        IList<(string NombreRuta, double[,] Coordenadas)> Result = await routesService.GetRoutesAsync(model, CancellationToken.None);

        _ = await Assert.That(Result).IsNotNull();
        _ = await Assert.That(Result.Any()).IsTrue();
    }
}
