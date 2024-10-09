using GoogleMapsLibrary.Core;
using GoogleMapsLibrary.Interfaces;
using GoogleMapsLibrary.Maps;
using GoogleMapsLibrary.Maps.Coordinates;
using Microsoft.JSInterop;
using OneOf;

namespace GoogleMapsLibrary.Marker;

/// <summary>
/// Shows a position on a map.
/// Note that the position must be set for the AdvancedMarkerElement to display.
/// Note: Usage as a Web Component (e.g. using the custom <code><gmp-advanced-marker></code> HTML element, is only available in the v=beta channel).
/// Custom element:
/// <code><!--<gmp-advanced-marker position="lat,lng" title="string"></gmp-advanced-marker>--></code>
/// This class extends HTMLElement.
/// This class implements AdvancedMarkerElementOptions.
/// </summary>
/// <see href="https://developers.google.com/maps/documentation/javascript/reference/advanced-markers#AdvancedMarkerElement"/>
public class AdvancedMarkerElement : MVCObject/*, IJsObjectRef*/
{
    //    public Guid Guid => _jsObjectRef.Guid;

    //    public static async Task<AdvancedMarkerElement> CreateAsync(IJSRuntime jsRuntime, AdvancedMarkerElementOptions? opts = null)
    //    {
    //        if (opts != null)
    //        {
    //            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    //            if (opts.Content.Value == null)
    //                opts.Content = "<div>&nbsp;</div>";
    //        }

    //        JsObjectRef jsObjectRef = await JsObjectRef.CreateAsync(jsRuntime, "google.maps.marker.AdvancedMarkerElement", opts);
    //        var obj = new AdvancedMarkerElement(jsObjectRef);
    //        return obj;
    //    }

    //    internal AdvancedMarkerElement(JsObjectRef jsObjectRef) : base(jsObjectRef) { }

    //    public Task<int> GetZIndex() => _jsObjectRef.InvokeAsync<int>("getZIndex");

    //    public async Task SetPosition(LatLngLiteral newPosition) => await _jsObjectRef.InvokePropertyAsync("position", newPosition);

    //    public async Task SetContent(OneOf<string, PinElement> newContent) => await _jsObjectRef.InvokePropertyAsync("content", newContent);

    //    public async Task SetGmpClickable(bool newValue) => await _jsObjectRef.InvokePropertyAsync("gmpClickable", newValue);

    //    public async Task<bool> GetGmpClickable() => await _jsObjectRef.InvokePropertyAsync<bool>("gmpClickable");

    //    public async Task SetGmpDraggable(bool newValue) => await _jsObjectRef.InvokePropertyAsync("gmpDraggable", newValue);

    //    public async Task<bool> GetGmpDraggable() => await _jsObjectRef.InvokePropertyAsync<bool>("gmpDraggable");

    //    public async Task<LatLngLiteral> GetPosition() => await _jsObjectRef.InvokePropertyAsync<LatLngLiteral>("position");

    //    public virtual Task<Map> GetMap() => _jsObjectRef.InvokeAsync<Map>("getMap");

    //    /// <summary>
    //    /// Renders the map entity on the specified map or panorama. 
    //    /// If map is set to null, the map entity will be removed.
    //    /// </summary>
    //    /// <param name="map"></param>
    //    public virtual async Task SetMap(Map? map) => await _jsObjectRef.InvokeAsync("setMap", map);

    //    public Task InvokeAsync(string functionName, params object[] args) => _jsObjectRef.InvokeAsync(functionName, args);

    //    public Task<T> InvokeAsync<T>(string functionName, params object[] args) => _jsObjectRef.InvokeAsync<T>(functionName, args);

    //    public Task<OneOf<T, U>> InvokeAsync<T, U>(string functionName, params object[] args) => _jsObjectRef.InvokeAsync<T, U>(functionName, args);

    //    public Task<OneOf<T, U, V>> InvokeAsync<T, U, V>(string functionName, params object[] args) => _jsObjectRef.InvokeAsync<T, U, V>(functionName, args);
}
