using Microsoft.JSInterop;

namespace GoogleMapsLibrary.Maps.Coordinates;

/// <summary>
/// Object literals are accepted in place of <see cref="LatLngBounds"> objects throughout the API.
/// These are automatically converted to <see cref="LatLngBounds"> objects.
/// All <see cref="South">, <see cref="West"/>, <see cref="North"/> and <see cref="East"/> must be set, otherwise an exception is thrown.
/// </summary>
/// <remarks>https://developers.google.com/maps/documentation/javascript/reference/coordinates#LatLngBoundsLiteral</remarks>
public class LatLngBoundsLiteral : LatLngBounds
{
    /// <summary>
    /// Constructs a new empty bounds.
    /// </summary>
    /// <param name="jsRuntime">Instance of a JavaScript runtime to which calls may be dispatched.</param>
    public static new async Task<LatLngBoundsLiteral> CreateAsync(IJSRuntime jsRuntime)
    {
        var obj = (LatLngBoundsLiteral)await LatLngBounds.CreateAsync(jsRuntime);

        return obj;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="jsRuntime">Instance of a JavaScript runtime to which calls may be dispatched.</param>
    /// <param name="latLngLiteral"></param>
    /// <returns></returns>
    public static new async Task<LatLngBoundsLiteral> CreateAsync(IJSRuntime jsRuntime, LatLngLiteral latLngLiteral)
    {
        var obj = (LatLngBoundsLiteral)await LatLngBounds.CreateAsync(jsRuntime, latLngLiteral);

        return obj;
    }

    protected LatLngBoundsLiteral(JsObjectRef jsObjectRef) : base(jsObjectRef) { }

    //protected LatLngBoundsLiteral(LatLngBounds latLngBounds) : base(latLngBounds) { }

    /// <summary>
    /// Default constructor. Set East, North, South and West explicitely because here they are initialized to zero.
    /// </summary>
    public LatLngBoundsLiteral() : base(default!) { }
    ///// <summary>
    ///// Constructor with one or two given coordinate points.
    ///// If the second point is null, the bounds are set to the first point.
    ///// The points may be positioned arbitrarily.
    ///// </summary>
    //public LatLngBoundsLiteral(JsObjectRef jsObjectRef, LatLng latLng1, LatLng? latLng2 = null) : base(jsObjectRef)
    //{
    //    East = latLng1.Lng;
    //    West = latLng1.Lng;
    //    South = latLng1.Lat;
    //    North = latLng1.Lat;
    //    if (latLng2 != null)
    //        _ = Extend(latLng2);
    //}

    ///// <summary>
    ///// Create or extend a LatLngBoundsLiteral with a given coordinate point.
    ///// Using this method you can initialize a LatLngBoundsLiteral reference with null and call 
    ///// subsequently this method to extend the boundaries by given points.
    ///// </summary>
    //public static void CreateOrExtend(ref LatLngBoundsLiteral? latLngBoundsLiteral, LatLngLiteral latLng)
    //{
    //    if (latLngBoundsLiteral == null)
    //        latLngBoundsLiteral = new LatLngBoundsLiteral(latLng);
    //    else
    //        _ = latLngBoundsLiteral.Extend(latLng);
    //}

    /// <summary>
    /// East longitude in degrees.
    /// Values outside the range [-180, 180] will be wrapped to the range [-180, 180). 
    /// For example, a value of -190 will be converted to 170. 
    /// A value of 190 will be converted to -170. 
    /// This reflects the fact that longitudes wrap around the globe.
    /// </summary>
    [JsonPropertyName("east")]
    public double East { get; set; }

    /// <summary>
    /// North latitude in degrees. Values will be clamped to the range [-90, 90]. 
    /// This means that if the value specified is less than -90, it will be set to -90. 
    /// And if the value is greater than 90, it will be set to 90.
    /// </summary>
    [JsonPropertyName("north")]
    public double North { get; set; }

    /// <summary>
    /// South latitude in degrees. Values will be clamped to the range [-90, 90]. 
    /// This means that if the value specified is less than -90, it will be set to -90. 
    /// And if the value is greater than 90, it will be set to 90.
    /// </summary>
    [JsonPropertyName("south")]
    public double South { get; set; }

    /// <summary>
    /// West longitude in degrees. 
    /// Values outside the range [-180, 180] will be wrapped to the range [-180, 180). 
    /// For example, a value of -190 will be converted to 170. 
    /// A value of 190 will be converted to -170. 
    /// This reflects the fact that longitudes wrap around the globe.
    /// </summary>
    [JsonPropertyName("west")]
    public double West { get; set; }

    ///// <summary>
    ///// Extend these boundaries by a given coordinate point.
    ///// </summary>
    //public void Extend(double lng, double lat)
    //{
    //    if (lng < West)
    //        West = lng;
    //    if (lng > East)
    //        East = lng;
    //    if (lat < South)
    //        South = lat;
    //    if (lat > North)
    //        North = lat;
    //}
    ///// <summary>
    ///// Extend these boundaries by a given coordinate point.
    ///// </summary>
    //public void Extend(decimal lng, decimal lat) => Extend(Convert.ToDouble(lng), Convert.ToDouble(lat));
    ///// <summary>
    ///// Extend these boundaries by a given coordinate point.
    ///// </summary>
    //public void Extend(LatLngLiteral latLng) => Extend(latLng.Lng, latLng.Lat);

    ///// <summary>
    ///// Is the area zero?
    ///// </summary>
    //public bool IsEmpty() => West == East || South == North;

    public override string ToString() => $"N:{North} E:{East} S:{South} W:{West}";
}
