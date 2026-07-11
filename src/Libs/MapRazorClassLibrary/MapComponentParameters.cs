using Microsoft.AspNetCore.Components;

namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent : IAsyncDisposable
{
    [Parameter] public EventCallback<MapComponent> OnMapCreatedAsync { get; set; }

    [Parameter] public EventCallback<MapClickEventArgs> OnMapClickAsync { get; set; }

    //[Parameter] public EventCallback<DragendMarkerEventArgs> OnDragendAsync { get; set; }

    [Parameter] public string Height { get; set; } = "500px";

    [Parameter] public string Width { get; set; } = "100%";

    [Parameter] public MapModels.MapOptions Options { get; set; } = new();
}
