using Microsoft.JSInterop;

namespace GoogleMapsLibrary.Maps.Coordinates;

/// <summary>
/// A <see cref="LatLngBounds"/> instance represents a rectangle in geographical coordinates, including one that crosses the 180 degrees longitudinal meridian.
/// </summary>
/// <remarks>https://developers.google.com/maps/documentation/javascript/reference/coordinates#LatLngBounds</remarks>
public class LatLngBounds /*: IDisposable*/
{
    protected internal readonly GmpJsInterop _jsObjectRef;

    /// <summary>
    /// Constructs a new empty bounds.
    /// </summary>
    /// <param name="jsRuntime">Instance of a JavaScript runtime to which calls may be dispatched.</param>
    public static async Task<LatLngBounds> CreateAsync(IJSRuntime jsRuntime)
    {
        GmpJsInterop jsObjectRef = (new GmpJsInterop(jsRuntime));/* await GmpJsInterop.CreateAsync(jsRuntime, "google.maps.LatLngBounds");*/

        LatLngBounds obj = new(jsObjectRef);

        //MaxBounds = await jsObjectRef.InvokePropertyAsync<LatLngBounds>("MAX_BOUNDS");

        return obj;
    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="jsRuntime">Instance of a JavaScript runtime to which calls may be dispatched.</param>
    //    /// <param name="latLngLiteral"></param>
    //    /// <returns></returns>
    //    public static async Task<LatLngBounds> CreateAsync(IJSRuntime jsRuntime, LatLngLiteral latLngLiteral)
    //    {
    //        LatLngBounds obj = await CreateAsync(jsRuntime);

    //        obj = await obj.Extend(latLngLiteral);

    //        return obj;
    //    }

    internal LatLngBounds() { }
    protected LatLngBounds(GmpJsInterop jsObjectRef) => _jsObjectRef = jsObjectRef;

    //    public void Dispose()
    //    {
    //        _jsObjectRef.Dispose();
    //        GC.SuppressFinalize(this);
    //    }

    //    /// <summary>
    //    /// LatLngBounds for the max bounds of the Earth. These bounds will encompass the entire globe.
    //    /// </summary>
    //    public static LatLngBounds MaxBounds { get; private set; } = default!;

    //    /// <summary>
    //    /// Returns true if the given lat/lng is in this bounds.
    //    /// </summary>
    //    public Task<bool> Contains(LatLngLiteral other) => _jsObjectRef.InvokeAsync<bool>("contains", other);

    //    /// <summary>
    //    /// Returns true if this bounds approximately equals the given bounds.
    //    /// </summary>
    //    public Task<bool> Equals(LatLngBoundsLiteral other) => _jsObjectRef.InvokeAsync<bool>("equals", other);

    //    /// <summary>
    //    /// Extends this bounds to contain the given point.
    //    /// </summary>
    //    public Task<LatLngBounds> Extend(LatLngLiteral point) => _jsObjectRef.InvokeAsync<LatLngBounds>("extend", point);

    //    /// <summary>
    //    /// Computes the center of this LatLngBounds
    //    /// </summary>
    //    public Task<LatLng> GetCenter() => _jsObjectRef.InvokeAsync<LatLng>("getCenter");

    //    /// <summary>
    //    /// Returns the north-east corner of this bounds.
    //    /// </summary>
    //    public Task<LatLng> GetNorthEast() => _jsObjectRef.InvokeAsync<LatLng>("getNorthEast");

    //    /// <summary>
    //    /// Returns the south-west corner of this bounds.
    //    /// </summary>
    //    public Task<LatLng> GetSouthWest() => _jsObjectRef.InvokeAsync<LatLng>("getSouthWest");

    //    /// <summary>
    //    /// Returns true if this bounds shares any points with the other bounds.
    //    /// </summary>
    //    public Task<bool> Intersects(LatLngBoundsLiteral other) => _jsObjectRef.InvokeAsync<bool>("intersects", other);

    //    /// <summary>
    //    /// Returns true if the bounds are empty.
    //    /// </summary>
    //    public Task<bool> IsEmpty() => _jsObjectRef.InvokeAsync<bool>("isEmpty");

    //    /// <summary>
    //    /// Returns the literal representation of this bounds
    //    /// </summary>
    //    public Task<LatLngBoundsLiteral> ToJson() => _jsObjectRef.InvokeAsync<LatLngBoundsLiteral>("toJSON");

    //    /// <summary>
    //    /// Converts the given map bounds to a lat/lng span.
    //    /// </summary>
    //    public Task<LatLng> ToSpan() => _jsObjectRef.InvokeAsync<LatLng>("toSpan");

    //    /// <summary>
    //    /// Returns a string of the form "lat_lo,lng_lo,lat_hi,lng_hi" for this bounds,
    //    /// where "lo" corresponds to the southwest corner of the bounding box, while "hi"
    //    /// corresponds to the northeast corner of that box.
    //    /// </summary>
    //    public Task<string> ToUrlValue(double precision) => _jsObjectRef.InvokeAsync<string>("toUrlValue", precision);

    //    /// <summary>
    //    /// Returns a string of the form "lat_lo,lng_lo,lat_hi,lng_hi" for this bounds,
    //    /// where "lo" corresponds to the southwest corner of the bounding box, while "hi"
    //    /// corresponds to the northeast corner of that box.
    //    /// </summary>
    //    public Task<string> ToUrlValue(decimal precision) => ToUrlValue(Convert.ToDouble(precision));

    //    /// <summary>
    //    /// Extends this bounds to contain the union of this and the given bounds.
    //    /// </summary>
    //    public Task<LatLngBounds> Union(LatLngBoundsLiteral other) => _jsObjectRef.InvokeAsync<LatLngBounds>("union", other);
}
