//using System.Diagnostics;

//namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels.Events;

//[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
//public sealed class PointerEvent : Event
//{
//    /// <summary>
//    /// The geographical point where the pointer event occurred.
//    /// </summary>
//    [J("latlng")] public required Basic.LatLng LatLng { get; init; }

//    /// <summary>
//    /// Pixel coordinates of the point where the pointer event occurred relative to the map layer.
//    /// </summary>
//    /*[J("layerPoint")]*/
//    public Basic.Point? LayerPoint { get; init; }

//    /// <summary>
//    /// Pixel coordinates of the point where the pointer event occurred relative to the map сontainer.
//    /// </summary>
//    /*[J("containerPoint")]*/
//    public Basic.Point? ContainerPoint { get; init; }

//    /// <summary>
//    /// The original <see href="https://developer.mozilla.org/en-US/docs/Web/API/MouseEvent">DOM MouseEvent</see> or <see href="https://developer.mozilla.org/en-US/docs/Web/API/PointerEvent">DOM PointerEvent</see> that triggered this Leaflet event.
//    /// </summary>
//    /*[J("originalEvent")]*/
//    public string? OriginalEvent { get; init; }

//    private string GetDebuggerDisplay() => $"{LatLng}";
//}
