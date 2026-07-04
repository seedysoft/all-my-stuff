using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Seedysoft.Libs.Travel.Services.Geocoding;

public class GeocodingService(IConfiguration configuration, ILogger<GeocodingService> logger) : ServiceBase(configuration)
{
    public async Task<IReadOnlyList<ViewModels.Place>> FindPlacesAsync(string textToFind, CancellationToken cancellationToken)
    {
        Settings.GeocodingServiceApi api = TravelSettings.GeocodingSettings.GeocodingApis.First(x => x.Name == TravelSettings.GeocodingSettings.CurrentImplName);

        Impl.GeocodingServiceImplBase GeocodingImpl = TravelSettings.GeocodingSettings.CurrentImplName switch
        {
#pragma warning disable format
            //Settings.GeocodingImplName.Google            => new GoogleRoutes(api, logger),
        
            //Settings.GeocodingImplName.MapboxDirections  => new MapboxDirectionsRouter(api, logger),
        
            Settings.GeocodingImplName.Nominatim         => new Impl.NominatimGeocodingServiceImpl(api, logger),

            Settings.GeocodingImplName.Photon            => new Impl.PhotonGeocodingServiceImpl(api, logger),
#pragma warning restore format

            _ => throw new InvalidOperationException($"Unsupported geocoder: {TravelSettings.GeocodingSettings.CurrentImplName}"),
        };

        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Using Geocoding impl: {GeocodingImpl}", TravelSettings.GeocodingSettings.CurrentImplName);

        return await GeocodingImpl.FindPlacesAsync(textToFind, cancellationToken);
    }
}
