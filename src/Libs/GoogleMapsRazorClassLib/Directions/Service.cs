using Microsoft.JSInterop;

namespace Seedysoft.Libs.GoogleMapsRazorClassLib.Directions;

/// <summary>
/// A service for computing directions between two or more places.
/// </summary>
public class Service(IJSRuntime jSRuntime, string elementId)
{
    /// <summary>
    /// Issue a directions search request.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<Result?> Route(Request request)
    {
        string response = await jSRuntime.InvokeAsync<string>($"{Constants.SeedysoftGoogleMaps}.directionsRoute", elementId, request);

        try
        {
            Result? dirResult = Serialization.Helper.DeSerializeObject<Result>(response);

            return dirResult;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error parsing DirectionsResult Object. Message: " + e.Message);
            return null;
        }
    }
}
