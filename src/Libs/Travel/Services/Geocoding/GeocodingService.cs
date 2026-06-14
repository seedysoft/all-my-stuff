using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Seedysoft.Libs.Travel.Services.Geocoding;

public class GeocodingService(IConfiguration configuration, ILogger<GeocodingService> logger) : ServiceBase(configuration)
{
    public async Task<IEnumerable<ViewModels.Place>> FindPlacesAsync(string textToFind, CancellationToken cancellationToken)
    {
        Settings.GeocodingApi api = TravelSettings.GeocodingSettings.GeocodingApis.First(x => x.Name == TravelSettings.GeocodingSettings.CurrentImplementation);

        GeocodingBase GeocodingImplementation = TravelSettings.GeocodingSettings.CurrentImplementation switch
        {
#pragma warning disable format
            //Settings.GeocodingImplementations.Google                     => new GoogleRoutes(api, logger),
            
            //Settings.GeocodingImplementations.MapboxDirections           => new MapboxDirectionsRouter(api, logger),
            
            Settings.GeocodingImplementations.Nominatim                  => new Nominatim(api, logger),
#pragma warning restore format

            _ => throw new InvalidOperationException($"Unsupported geocoder: {TravelSettings.GeocodingSettings.CurrentImplementation}"),
        };

        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Using Geocoding implementation: {GeocodingImplementation}", TravelSettings.GeocodingSettings.CurrentImplementation);

        return await GeocodingImplementation.FindPlacesAsync(textToFind, cancellationToken);
    }
}
