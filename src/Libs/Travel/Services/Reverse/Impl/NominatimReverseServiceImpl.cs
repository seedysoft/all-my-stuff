using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.Travel.Services.Reverse.Impl;

internal class NominatimReverseServiceImpl(
    IHttpClientFactory httpClientFactory
    , Settings.ReverseServiceApi api
    , ILogger logger) : ReverseServiceImplBase(httpClientFactory, api)
{
    // https://nominatim.openstreetmap.org/search?q={0}&format=json&limit=8
    internal async override Task<IReadOnlyList<ViewModels.Place>> FindPlacesAsync(Core.Models.Location location, CancellationToken cancellationToken)
    {
        try
        {
            if (Core.Models.Location.IsEmpty(location))
                return [];

            HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(Api.GetUrl(location), cancellationToken);

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
                    .Where(p => !string.IsNullOrWhiteSpace(p.DisplayName))
                    .Where(p => p.Lat != 0 && p.Lon != 0)
                    .Select(p => new ViewModels.Place(p.DisplayName!, new Core.Models.Location(p.Lat, p.Lon)))
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
        //public string category { get; init; }
        //public string type { get; init; }
        //public int place_rank { get; init; }
        //public float importance { get; init; }
        //public string addresstype { get; init; }
        //public string name { get; init; }
        [J("display_name")] public required string DisplayName { get; init; }
        public Address? Address { get; init; }
        //public string[] boundingbox { get; init; }
    }

    internal record Address
    {
        [J("house_number")] public string? HouseNumber { get; init; }
        [J("road")] public string? Road { get; init; }
        [J("neighbourhood")] public string? Neighbourhood { get; init; }
        [J("quarter")] public string? Quarter { get; init; }
        [J("suburb")] public string? Suburb { get; init; }
        [J("borough")] public string? Borough { get; init; }
        [J("city")] public string? City { get; init; }
        [J("province")] public string? Province { get; init; }
        [J("ISO31662lvl6")] public string? ISO31662lvl6 { get; init; }
        [J("state")] public string? State { get; init; }
        [J("ISO31662lvl4")] public string? ISO31662lvl4 { get; init; }
        [J("postcode")] public string? Postcode { get; init; }
        [J("country")] public string? Country { get; init; }
        [J("country_code")] public string? Country_code { get; init; }
    }
}
