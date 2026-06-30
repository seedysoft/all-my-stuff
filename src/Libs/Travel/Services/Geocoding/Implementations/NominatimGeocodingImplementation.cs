using RestSharp;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.Travel.Services.Geocoding.Implementations;

internal class NominatimGeocodingImplementation(Settings.GeocodingApi api, Microsoft.Extensions.Logging.ILogger logger) : GeocodingImplementationBase(api)
{
    // https://nominatim.openstreetmap.org/search?q={0}&format=json&limit=8
    internal async override Task<IReadOnlyList<ViewModels.Place>> FindPlacesAsync(string textToFind, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(textToFind))
                return [];

            RestClient restClient = new(new Uri(Api.UrlFormat).GetLeftPart(UriPartial.Authority));
            ResponseObject[]? restResponse = await restClient.GetAsync<ResponseObject[]>(Api.GetUrl(textToFind), cancellationToken);

            return restResponse == null
                ? []
                : [.. restResponse
                    .Where(p => !string.IsNullOrWhiteSpace(p.Display_name))
                    .Where(p => p.Lat != 0 && p.Lon != 0)
                    .Select(p => new ViewModels.Place(p.Display_name!, (decimal)p.Lat, (decimal)p.Lon))
                ];
        }
        catch (Exception e) when (logger.LogAndHandle(e, "Unexpected error")) { }

        return [];
    }
    internal record ResponseObject
    {
        //public int place_id { get; init; }
        //public string licence { get; init; }
        //public string osm_type { get; init; }
        //public int osm_id { get; init; }
        [J("lat")] public double Lat { get; init; }
        [J("lon")] public double Lon { get; init; }
        //public string _class { get; init; }
        //public string type { get; init; }
        //public int place_rank { get; init; }
        //public double importance { get; init; }
        //public string addresstype { get; init; }
        //public string name { get; init; }
        [J("display_name")] public required string Display_name { get; init; }
        //public double[] boundingbox { get; init; }
    }
}
