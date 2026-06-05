using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.Geography.Services;

public class PlacesService(IConfiguration configuration, ILogger<PlacesService> logger)
{
    protected Settings.GeographySettings GeographySettings => configuration
        .GetSection(nameof(Settings.GeographySettings)).Get<Settings.GeographySettings>()!;

    public async Task<IEnumerable<string>> FindPlacesAsync(
        string textToFind,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(textToFind))
                return [];

            RestClient restClient = new(new Uri(GeographySettings.PlacesApi.UrlFormat).GetLeftPart(UriPartial.Authority));
            Models.PlaceModel[]? restResponse = await restClient.GetAsync<Models.PlaceModel[]>(GeographySettings.PlacesApi.GetUrl(textToFind), cancellationToken);

            if (restResponse == null)
                return [];

            IEnumerable<string> places = restResponse
                .Where(p => !string.IsNullOrWhiteSpace(p.Address))
                .Select(p => p.Address!);

            return places.ToHashSet();
        }
        catch (Exception e) when (logger.LogAndHandle(e, "Unexpected error")) { }

        return [];
    }
}
