using Microsoft.AspNetCore.Components;

namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent
{
    [Parameter] public required string Height { get; set; } = "500px";

    [Parameter] public required string Width { get; set; } = "100%";

    /// <summary>
    /// Initial geographic center of the map.
    /// </summary>
    /// <remarks>Default: <c>undefined</c></remarks>
    [Parameter] public required MapModels.Basic.LatLng Center { get; set; }

    /// <summary>
    /// Initial map zoom level
    /// </summary>
    /// <remarks>Default: <c>undefined</c></remarks>
    [Parameter] public required double Zoom { get; set; }

    [Parameter] public EventCallback<MapComponent> OnMapCreatedAsyncEventCallback { get; set; }

    [Parameter] public EventCallback<MapClickEventArgs> OnMapClickAsyncEventCallback { get; set; }

    [Parameter] public EventCallback<MapModels.Basic.LatLngBounds> OnMapMoveEndEventCallback { get; set; }
}
