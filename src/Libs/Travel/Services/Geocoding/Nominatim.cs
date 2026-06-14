using RestSharp;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.Travel.Services.Geocoding;

internal class Nominatim(Settings.GeocodingApi api, Microsoft.Extensions.Logging.ILogger logger) : GeocodingBase(api)
{
    // https://nominatim.openstreetmap.org/search?q={0}&format=json&limit=8
    internal async override Task<IEnumerable<ViewModels.Place>> FindPlacesAsync(string textToFind, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(textToFind))
                return [];

            RestClient restClient = new(new Uri(Api.UrlFormat).GetLeftPart(UriPartial.Authority));
            Models.PlaceModel[]? restResponse = await restClient.GetAsync<Models.PlaceModel[]>(Api.GetUrl(textToFind), cancellationToken);

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

internal class NominatimApi : Settings.GeocodingApi
{
    public override string GetUrl(string text) => string.Format(UrlFormat, text);
}
