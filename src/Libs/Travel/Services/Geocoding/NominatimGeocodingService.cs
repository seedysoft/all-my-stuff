using RestSharp;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.Travel.Services.Geocoding;

internal class NominatimGeocodingService(Settings.GeocodingApi api, Microsoft.Extensions.Logging.ILogger logger) : GeocodingBase(api)
{
    // https://nominatim.openstreetmap.org/search?q={0}&format=json&limit=8
    internal async override Task<IList<ViewModels.Place>> FindPlacesAsync(string textToFind, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(textToFind))
                return [];

            RestClient restClient = new(new Uri(Api.UrlFormat).GetLeftPart(UriPartial.Authority));
            ResponseObject[]? restResponse = await restClient.GetAsync<ResponseObject[]>(Api.GetUrl(textToFind), cancellationToken);

            if (restResponse == null)
                return [];

            IEnumerable<ViewModels.Place> places = restResponse
                .Where(p => !string.IsNullOrWhiteSpace(p.Display_name))
                .Where(p => p.Lat != 0 && p.Lon != 0)
                .Select(p => new ViewModels.Place()
                {
                    Address = p.Display_name!,
                    Lat = (decimal)p.Lat,
                    Lon = (decimal)p.Lon
                });

            return [.. places];
        }
        catch (Exception e) when (logger.LogAndHandle(e, "Unexpected error")) { }

        return [];
    }
    public class ResponseObject
    {
        //public int place_id { get; set; }
        //public string licence { get; set; }
        //public string osm_type { get; set; }
        //public int osm_id { get; set; }
        [J("lat")] public double Lat { get; set; }
        [J("lon")] public double Lon { get; set; }
        //public string _class { get; set; }
        //public string type { get; set; }
        //public int place_rank { get; set; }
        //public double importance { get; set; }
        //public string addresstype { get; set; }
        //public string name { get; set; }
        [J("display_name")] public required string Display_name { get; set; }
        //public double[] boundingbox { get; set; }
    }
}
