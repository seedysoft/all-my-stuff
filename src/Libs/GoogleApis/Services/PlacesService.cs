using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.GoogleApis.Services;

public class PlacesService(IConfiguration configuration, ILogger<PlacesService> logger) : GoogleApisServiceBase(configuration)
{
    //public async Task<string> GetMapId(CancellationToken cancellationToken)
    //    => await Task.FromResult(GoogleApisSettings.MapsApi.MapId);

    public async Task<IEnumerable<string>> FindPlacesAsync(
        string textToFind,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(textToFind))
                return [];

            RestRequest restRequest = BuildFindPlacesRequest(textToFind);
            RestClient restClient = new(GoogleApisSettings.PlacesApi.UrlFormat);
            Models.Places.Response.Body? body = null;
            RestResponse restResponse = await restClient.ExecutePostAsync(restRequest, cancellationToken);
            if (restResponse.IsSuccessStatusCode)
                body = restResponse.Content!.FromJson<Models.Places.Response.Body>();
            if (body == null)
                return [];

            IEnumerable<string> places =
                from p in body.Suggestions.Select(x => x.PlacePrediction)
                where !string.IsNullOrWhiteSpace(p.Text?.Text)
                select p.Text!.Text;

            return places.ToArray();
        }
        catch (Exception e) when (logger.LogAndHandle(e, "Unexpected error")) { }

        return [];

        RestRequest BuildFindPlacesRequest(string textToFind)
        {
            RestRequest restRequest = new();
            restRequest = restRequest.AddHeader("X-Goog-Api-Key", GoogleApisSettings.ApiKey);
            restRequest = restRequest.AddHeader("X-Goog-FieldMask", GoogleApisSettings.FieldMask);

            Models.Places.Request.Body PlacesRequestBody = new()
            {
                // Required
                Input = textToFind,
                // Optional
                IncludedPrimaryTypes = ["geocode", "locality", "route", "street_address"],
                IncludeQueryPredictions = false,
                //IncludedRegionCodes = [""],
                //LanguageCode = "",
                //LocationBias = new()
                //{
                //    Rectangle = new()
                //    {
                //        High = new() { Latitude = 1.1, Longitude = 1.1, },
                //        Low = new() { Latitude = 2.2, Longitude = 2.2, },
                //    }
                //},
                //LocationRestriction = new()
                //{
                //    Circle = new() { Center = new() { Latitude = 3.3, Longitude = 3.3, }, }
                //},
                //RegionCode = "",
                //Origin = new() { Latitude = 5.5, Longitude = 5.5, },
                //SessionToken = string.Empty,
            };

            return restRequest.AddJsonBody(PlacesRequestBody.ToJson(), ContentType.Json);
        }
    }
}
