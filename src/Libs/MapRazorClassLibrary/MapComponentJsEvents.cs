using Microsoft.JSInterop;

namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent : IAsyncDisposable
{
    //[JSInvokable]
    //public async Task OnDragend(DragendMarkerInternalEventArgs e)
    //{
    //    if (OnDragendAsync.HasDelegate)
    //    {
    //        var publicArgs = new DragendMarkerEventArgs
    //        {
    //            MarkerId = e.MarkerId,
    //            Point = e.Point
    //        };
    //        await OnDragendAsync.InvokeAsync(publicArgs);
    //    }
    //}

    //[JSInvokable]
    //public async Task OnMapClick(double? lat, double? lng, string address, AddressDetails? placeDetails, string? markerId)
    //{
    //    if (OnMapClickAsync.HasDelegate)
    //    {
    //        PositionPoint point = PositionPoint.CreateFromCoordinates(lat, lng);
    //        MapClickEventArgs place = new MapClickEventArgs(markerId, address, point, placeDetails);
    //        await OnMapClickAsync.InvokeAsync(place);
    //    }
    //}
}

//public class MapClickEventArgs(string markerId, string address, LatLong point = null, IAddressDetails details = null)
//{
//    public string MarkerId => markerId;
//    public LatLong Point => point;
//    public string Address => address;
//    public IAddressDetails Details => details;
//}

//public class PositionPoint : LatLong, IEquatable<LatLong>
//{
//    public double Latitude { get; set; }
//    public double Longitude { get; set; }
//    public PositionPoint(double latitude, double longitude)
//    {
//        ValidateCoordinates(latitude, longitude);
//        Latitude = latitude;
//        Longitude = longitude;
//    }

//    public static PositionPoint? CreateFromCoordinates(double? lat, double? lng)
//    {
//        if (lat is null || lng is null)
//        {
//            return null;
//        }

//        try
//        {
//            return new PositionPoint((float)lat, (float)lng);
//        }
//        catch
//        {
//            return null;
//        }
//    }

//    private static void ValidateCoordinates(double latitude, double longitude)
//    {
//        if (double.IsNaN(latitude) || double.IsInfinity(latitude))
//            throw new ArgumentException("Latitude cannot be NaN or infinity.", nameof(latitude));
//        if (double.IsNaN(longitude) || double.IsInfinity(longitude))
//            throw new ArgumentException("Longitude cannot be NaN or infinity.", nameof(longitude));
//        if (latitude < -90 || latitude > 90)
//            throw new ArgumentException("The latitude must be between - 90 and 90 degrees.", nameof(latitude));
//        if (longitude < -180 || longitude > 180)
//            throw new ArgumentException("The longitude must be between -180 and 180 degrees.", nameof(longitude));
//    }

//    public bool Equals(LatLong? other) => Latitude == other?.Latitude && Longitude == other?.Longitude;
//    public override bool Equals(object? obj) => obj is LatLong other && Equals(other);
//    public override int GetHashCode() => HashCode.Combine(Latitude, Longitude);
//    public static bool operator ==(PositionPoint left, PositionPoint right) => left.Equals(right);
//    public static bool operator !=(PositionPoint left, PositionPoint right) => !left.Equals(right);
//    public override string ToString() => $"Latitude: {Latitude}, Longitude: {Longitude}";
//}
