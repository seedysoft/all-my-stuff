using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.Travel.Services;

internal class NominatimService(HttpClient httpClient, ILogger<NominatimService> logger) :
    Interfaces.IGeocodingService,
    Interfaces.IReverseService
{
    public async Task<IReadOnlyList<ViewModels.Place>> GeocodeAsync(
        string textToFind,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(textToFind))
                return [];

            HttpRequestMessage httpRequestMessage = new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://nominatim.openstreetmap.org/search?q={textToFind}&format=json&featureType=settlement"),
            };

            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                ResponseObject[]? body = await httpResponseMessage.Content.ReadFromJsonAsync<ResponseObject[]>(cancellationToken);

                return [.. body?
                    .Where(p => !string.IsNullOrWhiteSpace(p.DisplayName))
                    .Where(p => p.Latitude != 0 && p.Longitude != 0)
                    .Select(p => new ViewModels.Place(p.DisplayName!, new Core.Models.Location(p.Latitude, p.Longitude))) ?? []
                    ];
            }
            else
            {
                if (logger.IsEnabled(LogLevel.Warning))
                    logger.LogWarning(nameof(NominatimService) + "." + nameof(GeocodeAsync) + ": {StatusCode} {ReasonPhrase}", httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase);
            }
        }
        catch (Exception e) when (e is OperationCanceledException || e.InnerException is OperationCanceledException) { throw; }
        catch (Exception e) when (logger.LogAndHandle(e, "Unexpected error")) { }

        return [];
    }

    // https://nominatim.openstreetmap.org/reverse?lat={Lat}&lon={Lon}&format={For}&addressdetails={Add}&extratags={Ext}&namedetails={Nam}&entrances={Ent}&zoom={Zoo}&layer={Lay}
    public async Task<ViewModels.Place> ReverseAsync(
        Core.Models.Location location,
        CancellationToken cancellationToken)
    {
        try
        {
            if (Core.Models.Location.IsEmpty(location))
                return ViewModels.Place.Empty;

            HttpRequestMessage httpRequestMessage = new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://nominatim.openstreetmap.org/reverse"),
            };
            _ = httpRequestMessage.Options.TryAdd("lat", location.Latitude);
            _ = httpRequestMessage.Options.TryAdd("lon", location.Longitude);
            _ = httpRequestMessage.Options.TryAdd("format", "json");
            _ = httpRequestMessage.Options.TryAdd("addressdetails", 0);

            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                ResponseObject? body = await httpResponseMessage.Content.ReadFromJsonAsync<ResponseObject>(cancellationToken);

                if (body != null)
                    return new ViewModels.Place(body.DisplayName!, new Core.Models.Location(body.Latitude, body.Longitude));
            }
            else
            {
                if (logger.IsEnabled(LogLevel.Warning))
                    logger.LogWarning(nameof(NominatimService) + ": " + nameof(ReverseAsync) + ": {StatusCode} {ReasonPhrase}", httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase);
            }
        }
        catch (Exception e) when (e is OperationCanceledException || e.InnerException is OperationCanceledException) { throw; }
        catch (Exception e) when (logger.LogAndHandle(e, "Unexpected error")) { }

        return ViewModels.Place.Empty;
    }

    internal record ResponseObject
    {
        //[J("place_id")] public long PlaceId { get; init; }
        //[J("licence")] public required string Licence { get; init; }
        //[J("osm_type")] public required string OsmType { get; init; }
        //[J("osm_id")] public long OsmId { get; init; }
        [J("lat")] public required double Latitude { get; init; }
        [J("lon")] public required double Longitude { get; init; }
        //[J("class")] public required string Class { get; init; }
        //[J("type")] public required string Type { get; init; }
        //[J("place_rank")] public int PlaceRank { get; init; }
        //[J("importance")] public double Importance { get; init; }
        //[J("addresstype")] public required string AddressType { get; init; }
        [J("name")] public required string Name { get; init; }
        [J("display_name")] public required string DisplayName { get; init; }
        //[J("boundingbox")] public required double[] BoundingBox { get; init; }
    }
}
