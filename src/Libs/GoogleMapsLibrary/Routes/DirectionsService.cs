//using Microsoft.JSInterop;

//namespace GoogleMapsLibrary.Routes;

///// <summary>
///// A service for computing directions between two or more places.
///// </summary>
//public class DirectionsService : IDisposable
//{
//    private readonly JsObjectRef _jsObjectRef;

//    /// <summary>
//    /// Creates a new instance of a DirectionsService that sends directions queries to Google servers.
//    /// </summary>
//    public static async Task<DirectionsService> CreateAsync(IJSRuntime jsRuntime)
//    {
//        JsObjectRef jsObjectRef = await JsObjectRef.CreateAsync(jsRuntime, "google.maps.DirectionsService");

//        var obj = new DirectionsService(jsObjectRef);

//        return obj;
//    }

//    /// <summary>
//    /// Creates a new instance of a DirectionsService that sends directions queries to Google servers.
//    /// </summary>
//    private DirectionsService(JsObjectRef jsObjectRef) => _jsObjectRef = jsObjectRef;

//    public void Dispose() => _jsObjectRef.Dispose();

//    /// <summary>
//    /// Issue a directions search request.
//    /// </summary>
//    /// <param name="request"></param>
//    /// <param name="directionsRequestOptions">Lets you specify which route response paths to opt out from clearing.</param>
//    /// <returns></returns>
//    public async Task<DirectionsResult?> Route(DirectionsRequest request, DirectionsRequestOptions? directionsRequestOptions = null)
//    {
//        directionsRequestOptions ??= new DirectionsRequestOptions();

//        string response = await _jsObjectRef.InvokeAsync<string>("blazorGoogleMaps.directionService.route", request, directionsRequestOptions);

//        try
//        {
//            DirectionsResult? dirResult = Serialization.Helper.DeSerializeObject<DirectionsResult>(response);
//            return dirResult;
//        }
//        catch (Exception e)
//        {
//            Console.WriteLine("Error parsing DirectionsResult Object. Message: " + e.Message);
//            return null;
//        }
//    }
//}
