using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.GasStationPrices.Services;

public class PlacesService(IConfiguration configuration, ILogger<PlacesService> logger) : GeographyServiceBase(configuration)
{
    public async Task<IEnumerable<ViewModels.Place>> FindPlacesAsync(string textToFind, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(textToFind))
                return [];

            RestClient restClient = new(new Uri(GeographySettings.PlacesApi.UrlFormat).GetLeftPart(UriPartial.Authority));
            Models.PlaceModel[]? restResponse = await restClient.GetAsync<Models.PlaceModel[]>(GeographySettings.PlacesApi.GetUrl(textToFind), cancellationToken);

            if (restResponse == null)
                return [];

            IEnumerable<ViewModels.Place> places = restResponse
                .Where(p => !string.IsNullOrWhiteSpace(p.Address))
                .Where(p => p.Lat != 0 && p.Lng != 0)
                .Select(p => new ViewModels.Place()
                {
                    Address = p.Address!,
                    Latitude = p.Lat,
                    Longitude = p.Lng
                });

            return places.ToHashSet();
        }
        catch (Exception e) when (logger.LogAndHandle(e, "Unexpected error")) { }

        return [];
    }
}
