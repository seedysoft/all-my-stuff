using Microsoft.JSInterop;

namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent
{
    public sealed class MapClickEventArgs : EventArgs
    {
        [J("latLng")] public required MapModels.Basic.LatLng LatLng { get; set; }
    }

    [JSInvokable]
    public async Task OnMapClickAsync(MapClickEventArgs args)
        => await OnMapClickAsyncEventCallback.InvokeAsync(args);

    public sealed class MapDragendEventArgs : EventArgs
    {

    }

    [JSInvokable]
    public async Task OnMapDragendAsync(MapDragendEventArgs args)
        => await OnMapDragendAsyncEventCallback.InvokeAsync(args);

    /// <summary>
    /// Initializes the component by creating a DotNet object reference for JavaScript interop.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This lifecycle method is called automatically by Blazor once during the component initialization phase.
    /// It creates a DotNetObjectReference that enables JavaScript code (in MapComponent.js) to invoke .NET methods
    /// on this component instance, establishing the bidirectional communication bridge.
    /// </para>
    /// <para>
    /// The reference is stored in the ObjRef property for later cleanup during disposal.
    /// </para>
    /// <para>
    /// Timing: This runs before OnAfterRenderAsync, ensuring the reference is available for all subsequent operations.
    /// </para>
    /// </remarks>
    /// <seealso cref="OnAfterRenderAsync"/>
    protected override void OnInitialized() => ObjRef = DotNetObjectReference.Create(this);

    /// <summary>
    /// Initializes the map module and creates the map instance on first render.
    /// </summary>
    /// <param name="firstRender">Indicates whether this is the first render pass of the component.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <remarks>
    /// <para>
    /// This lifecycle method is invoked by Blazor after the component has been rendered to the browser DOM.
    /// It is called twice per render cycle: once after the initial render and once after re-renders.
    /// </para>
    /// <para>
    /// Execution Flow on First Render:
    /// <list type="number">
    /// <item><description>Calls the base class OnAfterRenderAsync to ensure inheritance chain is maintained</description></item>
    /// <item><description>Checks if this is the first render pass; skips execution on subsequent renders for performance</description></item>
    /// <item><description>Verifies that MapModule has not already been initialized (null check)</description></item>
    /// <item><description>Dynamically imports the MapComponent.js from the component's JavaScript folder using ES6 modules</description></item>
    /// <item><description>Invokes CreateMapAsync() to initialize the Leaflet map instance with default settings</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Performance Optimization:
    /// Returns immediately on non-first renders, preventing unnecessary module imports and map initialization.
    /// The map is only created once during the component's lifetime.
    /// </para>
    /// <para>
    /// Module Path:
    /// The MapComponent.js is imported from _content/Seedysoft.Libs.MapRazorClassLibrary/js/, which is the standard
    /// path for static files in Razor Class Libraries served to Blazor WebAssembly applications.
    /// </para>
    /// </remarks>
    /// <seealso cref="OnInitialized"/>
    /// <seealso cref="CreateMapAsync"/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
            return;

        if (MapModule == null)
        {
            MapModule = await JsRuntime.InvokeAsync<IJSObjectReference>(
                identifier: "import",
                args: $"./{Assets["_content/Seedysoft.Libs.MapRazorClassLibrary/js/MapComponent.js"]}");

            await CreateMapAsync();
        }
    }
}
