using Microsoft.AspNetCore.Components;

namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent : IAsyncDisposable
{
    [Inject] MapService LeafletService { get; set; }

    [Parameter] public ILatLong OriginalPoint { get; set; } = new LatLong(15.192939, 120.586715);
    [Parameter] public byte ZoomLevel { get; set; } = 19;
    [Parameter] public EventCallback<MapComponent> OnMapCreatedAsync { get; set; }
    [Parameter] public EventCallback<MapClickEventArgs> OnMapClickAsync { get; set; }
    [Parameter] public EventCallback<DragendMarkerEventArgs> OnDragendAsync { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> Attributes { get; set; }
}
