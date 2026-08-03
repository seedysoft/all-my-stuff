using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.Travel.Services.Geocoding.Impl;

internal class NominatimGeocodingServiceImpl(
    IHttpClientFactory httpClientFactory
    , Settings.GeocodingServiceApi api
    , ILogger logger) : GeocodingServiceImplBase(httpClientFactory, api)
{
    // https://nominatim.openstreetmap.org/search?q={0}&format=json&limit=8
    internal async override Task<IReadOnlyList<ViewModels.Place>> FindPlacesAsync(string textToFind, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(textToFind))
                return [];

            HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(Api.GetUrl(textToFind), cancellationToken);

            ResponseObject[]? body = null;
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                body = await httpResponseMessage.Content.FromJsonAsync<ResponseObject[]>(cancellationToken);
            }
            else
            {
                if (logger.IsEnabled(LogLevel.Warning))
                    logger.LogWarning("ValhallaRoutingServiceImpl: FindPlacesAsync: {StatusCode} {ReasonPhrase}", httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase);
            }

            return body == null
                ? []
                : [.. body
                    .Where(p => !string.IsNullOrWhiteSpace(p.Display_name))
                    .Where(p => p.Lat != 0 && p.Lon != 0)
                    .Select(p => new ViewModels.Place(p.Display_name!, new Models.Location(p.Lat, p.Lon)))
                ];
        }
        catch (Exception e) when (e is OperationCanceledException || e.InnerException is OperationCanceledException) { throw; }
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
