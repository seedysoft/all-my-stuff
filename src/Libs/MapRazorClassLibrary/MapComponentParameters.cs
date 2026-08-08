using Microsoft.AspNetCore.Components;

namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent : IAsyncDisposable
{
    [Parameter] public EventCallback<MapComponent> OnMapCreatedAsyncEventCallback { get; set; }

    [Parameter] public EventCallback<MapClickEventArgs> OnMapClickAsyncEventCallback { get; set; }

    [Parameter] public EventCallback<MapDragendEventArgs> OnMapDragendAsyncEventCallback { get; set; }

    [Parameter] public string Height { get; set; } = "500px";

    [Parameter] public string Width { get; set; } = "100%";

    [Parameter] public MapModels.Map Map { get; set; } = new();
}
