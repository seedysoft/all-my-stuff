//using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using RestSharp;
//using Seedysoft.Libs.Core.Extensions;

//namespace Seedysoft.Libs.GoogleApis.Services;

///// <summary>
///// A service for computing directions between two or more places.
///// </summary>
//public class DirectionsService(IConfiguration configuration, ILogger<DirectionsService> logger) : GoogleApisServiceBase(configuration)
//{
//    //    /// <summary>
//    //    /// Issue a directions search request.
//    //    /// </summary>
//    //    /// <param name="request"></param>
//    //    /// <returns></returns>
//    //    //public async Task<GoogleApis.Models.DirectionsServiceRoutes[]> RouteAsync(
//    //    //    GoogleApis.Models.Directions.Request.Body request)
//    //    public async Task<Models.Directions.Response.Body> RouteAsync(
//    //        Models.Directions.Request.Body request)
//    //    {
//    //        try
//    //        {
//    //            //return await jSRuntime.InvokeAsync<GoogleApis.Models.DirectionsServiceRoutes[]>(
//    //            return await JSRuntime.InvokeAsync<Models.Directions.Response.Body>(
//    //                $"{Constants.SeedysoftGoogleMaps}.directionsRoute",
//    //#if DEBUG
//    //                TimeSpan.FromSeconds(15),
//    //#else
//    //                TimeSpan.FromSeconds(2),
//    //#endif
//    //                ElementId, request, objRef);
//    //        }
//    //        catch (Exception e) { Console.WriteLine("Error parsing DirectionsResult Object. Message: " + e.Message); }

//    //        return default!;
//    //    }

//    public async Task<Models.Directions.Response.Body?> RouteAsync(string origin, string destination)
//    {
//        try
//        {
//            if (string.IsNullOrWhiteSpace(origin) || string.IsNullOrWhiteSpace(destination))
//                return default;

//            Models.Directions.Response.Body? body = null;
//            string resourceUrl = string.Format(GoogleApisSettings.DirectionsApi.ResourceUrlFormat, GoogleApisSettings.ApiKey, origin, destination);
//            RestRequest restRequest = BuildDirectionsRequest(origin, destination);
//            RestClient restClient = new(GoogleApisSettings.DirectionsApi.BaseUrl)
//            {
//                AcceptedContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json,],
//            };
//            RestResponse restResponse = await restClient.ExecuteGetAsync(restRequest);
//            if (restResponse.IsSuccessStatusCode)
//                body = restResponse.Content!.FromJson<Models.Directions.Response.Body>();

//            return body;
//        }
//        catch (Exception e) when (logger.LogAndHandle(e, "Unexpected error")) { }

//        return default;

//        RestRequest BuildDirectionsRequest(string origin, string destination)
//        {
//            RestRequest restRequest = new();
//            restRequest = restRequest.AddHeader("X-Goog-Api-Key", GoogleApisSettings.ApiKey);

//            Models.Directions.Request.Body RoutesRequestBody = new()
//            {
//                Origin = origin,
//                Destination = destination,
//                ProvideRouteAlternatives = true,
//                TravelMode = Models.Directions.Shared.TravelMode.Driving,
//            };

//            return restRequest.AddJsonBody(RoutesRequestBody.ToJson(), ContentType.Json);
//        }
//    }
//}
