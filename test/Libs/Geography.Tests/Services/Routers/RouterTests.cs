using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seedysoft.Libs.Infrastructure.Extensions;

namespace Seedysoft.Libs.Geography.Tests.Services.Routers;

public sealed class RouterTests : Infrastructure.Tests.TestClassBase
{
    private readonly Geography.Services.RoutesService routesService = default!;

    public RouterTests() : base()
    {
        HostApplicationBuilder appBuilder = new();
        _ = appBuilder.AddAllMyDependencies();
        ServiceProvider serviceProvider = appBuilder.Services.BuildServiceProvider();

        routesService = serviceProvider.GetRequiredService<Geography.Services.RoutesService>();
    }

    [Test]
    public async Task GetLatestReleaseFromGithubAsyncTest()
    {
        ViewModels.TravelQueryModel model = new()
        {
            Origin = new ViewModels.Place()
            {
                Address = "Calle Juan Ramon Jimenez, 8, Burgos, Spain",
                Latitude = 42.354358f,
                Longitude = -3.662786f,
            },
            Destination = new ViewModels.Place()
            {
                Address = "Calle Iglesia 11, 24715 Brazuelo, León",
                Latitude = 42.541333f,
                Longitude = -6.194499f
            },
            MaxDistanceInKm = 10,
            PetroleumProductsSelectedIds = GasStationPrices.Models.Minetur.ProductoPetrolifero.Gasoline.Select(static x => x.IdProducto).ToHashSet(),
        };

        IList<(string NombreRuta, double[][] Coordenadas)> Result = await routesService.GetRoutesAsync(model, CancellationToken.None);

        _ = await Assert.That(Result).IsNotNull();
    }
}
