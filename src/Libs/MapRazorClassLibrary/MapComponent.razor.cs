using Microsoft.JSInterop;

// TODO                                                                             Fix blinking map

namespace Seedysoft.Libs.MapRazorClassLibrary;

/// <summary>
/// Blazor component that manages an interactive map with route visualization and gas station markers.
/// Integrates Leaflet.js for map rendering and provides functionality to display travel routes and nearby gas stations.
/// </summary>
/// <remarks>
/// <para>
/// This component is a wrapper around the Leaflet.js mapping library, designed to provide a rich interactive map experience
/// in Blazor WebAssembly applications. It serves as a visual interface for route planning and gas station discovery.
/// </para>
/// <para>
/// Key Features:
/// <list type="bullet">
/// <item><description>Dynamically loads a Leaflet map module on first render for lazy loading optimization</description></item>
/// <item><description>Displays multiple travel routes with distinct colors for easy differentiation</description></item>
/// <item><description>Marks gas stations with color-coded circle indicators based on fuel price comparisons (minimum, average, maximum)</description></item>
/// <item><description>Supports asynchronous cancellation tokens for all operations to enable responsive UI cancellation</description></item>
/// <item><description>Implements IAsyncDisposable for proper cleanup of JavaScript interop resources and event listeners</description></item>
/// <item><description>Uses JavaScript interop to invoke Leaflet library features while maintaining type safety on the C# side</description></item>
/// </list>
/// </para>
/// <para>
/// Architecture:
/// The component uses local JavaScript interop through a dynamically imported MapComponent.js file that handles all direct Leaflet
/// library interactions. This separation of concerns allows for maintainable and testable code while leveraging Leaflet's powerful
/// mapping capabilities.
/// </para>
/// <para>
/// Usage:
/// The component is initialized via the LoadRoutesAndGasStationsAsync method, which orchestrates the entire data loading pipeline.
/// The component manages its own state internally, including markers, polylines, and the map instance.
/// </para>
/// </remarks>
public partial class MapComponent
{
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
