//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Seedysoft.Libs.Infrastructure.Extensions;
//using Xunit;

//namespace Seedysoft.Libs.GoogleApis.Tests.Services;

//public sealed class DirectionsServiceTests : Infrastructure.Tests.TestClassBase
//{
//    private readonly GoogleApis.Services.DirectionsService DirectionsService = default!;

//    public DirectionsServiceTests() : base()
//    {
//        HostApplicationBuilder appBuilder = new();
//        _ = appBuilder.AddAllMyDependencies();
//        ServiceProvider serviceProvider = appBuilder.Services.BuildServiceProvider();

//        DirectionsService = serviceProvider.GetRequiredService<GoogleApis.Services.DirectionsService>();
//    }

//    [InlineData(["Juan Ramón Jiménez 8 Burgos", "Calle de la Iglesia 11 Brazuelo León"])]
//    [Theory]
//    public async Task RouteAsyncTest(string origin, string destination)
//    {
//        Models.Directions.Response.Body? Res =
//              await DirectionsService.RouteAsync(origin, destination);

//        Assert.NotNull(Res);
//    }

//    protected override void Dispose(bool disposing) => Dispose();
//}
