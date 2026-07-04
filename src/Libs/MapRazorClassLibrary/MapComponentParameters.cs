using Microsoft.AspNetCore.Components;

namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent : IAsyncDisposable
{
    [Inject] private MapService LeafletService { get; set; } = default!;

    [Inject] private GasStationPrices.Services.GasStationPricesService GasStationPricesService { get; set; } = default!;

    [Inject] private Travel.Services.Routing.RoutingService RoutingService { get; set; } = default!;

    [Parameter] public Travel.Models.Location OriginalPoint { get; set; } = Travel.Constants.Earth.Burgos;
    
    [Parameter] public byte ZoomLevel { get; set; } = 14;
    
    [Parameter] public EventCallback<MapComponent> OnMapCreatedAsync { get; set; }
    //[Parameter] public EventCallback<MapClickEventArgs> OnMapClickAsync { get; set; }
    //[Parameter] public EventCallback<DragendMarkerEventArgs> OnDragendAsync { get; set; }

    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> Attributes { get; set; } = [];
}
