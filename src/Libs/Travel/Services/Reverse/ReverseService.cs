using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Seedysoft.Libs.Travel.Services.Reverse;

public class ReverseService(
    IServiceProvider serviceProvider
    , IConfiguration configuration
    , ILogger<ReverseService> logger) : ServiceBase(serviceProvider, configuration)
{
    public async Task<IReadOnlyList<ViewModels.Place>> FindPlacesAsync(Core.Models.Location location, CancellationToken cancellationToken)
    {
        Settings.ReverseServiceApi api = TravelSettings.ReverseSettings.ReverseApis.First(x => x.Name == TravelSettings.ReverseSettings.CurrentImplName);

        Impl.ReverseServiceImplBase ReverseImpl = TravelSettings.ReverseSettings.CurrentImplName switch
        {
#pragma warning disable format
            //Settings.ReverseImplName.MapboxDirections  => new MapboxDirectionsRouter(api, logger),

            Settings.ReverseImplName.Nominatim         => new Impl.NominatimReverseServiceImpl( HttpClientFactory, api, logger),
#pragma warning restore format

            _ => throw new InvalidOperationException($"Unsupported reverse geocoder: {TravelSettings.ReverseSettings.CurrentImplName}"),
        };

        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Using Reverse impl: {ReverseImpl}", TravelSettings.ReverseSettings.CurrentImplName);

        return await ReverseImpl.FindPlacesAsync(location, cancellationToken);
    }
}
