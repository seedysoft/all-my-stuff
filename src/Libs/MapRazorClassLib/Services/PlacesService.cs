using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.MapRazorClassLib.Services;

public class PlacesService(IConfiguration configuration, ILogger<PlacesService> logger)
{
    protected Settings.MapRazorClassLibSettings MapRazorClassLibSettings => configuration
        .GetSection(nameof(Settings.MapRazorClassLibSettings)).Get<Settings.MapRazorClassLibSettings>()!;

    public async Task<IEnumerable<string>> FindPlacesAsync(
        string textToFind,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(textToFind))
                return [];

            RestClient restClient = new(MapRazorClassLibSettings.PlacesApi.UrlFormat);
            Models.PlaceModel[]? restResponse = await restClient.GetAsync<Models.PlaceModel[]>(string.Format(MapRazorClassLibSettings.PlacesApi.UrlFormat, textToFind), cancellationToken);

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
