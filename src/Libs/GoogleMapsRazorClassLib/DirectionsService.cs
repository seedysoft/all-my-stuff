using Microsoft.JSInterop;

namespace Seedysoft.Libs.GoogleMapsRazorClassLib;

/// <summary>
/// A service for computing directions between two or more places.
/// </summary>
public class DirectionsService(IJSRuntime jSRuntime, string elementId)
{
    /// <summary>
    /// Issue a directions search request.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    //public async Task<GoogleApis.Models.Shared.LatLngBoundsLiteral> RouteAsync(GoogleApis.Models.Directions.Request.Body request)
    public async Task RouteAsync(GoogleApis.Models.Directions.Request.Body request)
    {
        try
        {
            /*return await jSRuntime.InvokeAsync<GoogleApis.Models.Shared.LatLngBoundsLiteral>(*/
            await jSRuntime.InvokeVoidAsync(
                $"{Constants.SeedysoftGoogleMaps}.directionsRoute",
                TimeSpan.FromSeconds(5),
                [elementId, request]);
        }
        catch (Exception e) { Console.WriteLine("Error parsing DirectionsResult Object. Message: " + e.Message); }

        //return default!;
    }
}
