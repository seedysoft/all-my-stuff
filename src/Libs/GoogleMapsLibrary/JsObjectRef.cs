using GoogleMapsLibrary.Helpers;
using GoogleMapsLibrary.Interfaces;
using Microsoft.JSInterop;
using OneOf;

namespace GoogleMapsLibrary;

public class JsObjectRef(IJSRuntime jsRuntime, Guid guid) : IJsObjectRef, IDisposable
{
    public Guid Guid { get; private set; } = guid;
    public IJSRuntime JSRuntime { get; private set; } = jsRuntime;

    public static Task<JsObjectRef> CreateAsync(IJSRuntime jsRuntime, string constructorFunctionName, params object?[] args)
        => CreateAsync(jsRuntime, Guid.NewGuid(), constructorFunctionName, args);

    public static async Task<Dictionary<string, JsObjectRef>> CreateMultipleAsync(
        IJSRuntime jsRuntime,
        string constructorFunctionName,
        Dictionary<string, object> args)
    {
        var internalMapping = args.ToDictionary(e => e.Key, e => Guid.NewGuid());
        var dictArgs = internalMapping.ToDictionary(e => e.Value, e => args[e.Key]);
        Dictionary<Guid, JsObjectRef> result = await CreateMultipleAsync(
            jsRuntime,
            constructorFunctionName,
            dictArgs);

        return internalMapping.ToDictionary(e => e.Key, e => result[e.Value]);
    }

    public async Task<Dictionary<string, JsObjectRef>> AddMultipleAsync(
        string constructorFunctionName,
        Dictionary<string, object> args)
    {
        var internalMapping = args.ToDictionary(e => e.Key, _ => Guid.NewGuid());
        var dictArgs = internalMapping.ToDictionary(e => e.Value, e => args[e.Key]);
        Dictionary<Guid, JsObjectRef> result = await CreateMultipleAsync(
            JSRuntime,
            constructorFunctionName,
            dictArgs);

        return internalMapping.ToDictionary(e => e.Key, e => result[e.Value]);
    }

    public async static Task<JsObjectRef> CreateAsync(
        IJSRuntime jsRuntime,
        Guid guid,
        string functionName,
        params object?[] args)
    {
        var jsObjectRef = new JsObjectRef(jsRuntime, guid);

        _ = await jsRuntime.MyInvokeAsync<object>("blazorGoogleMaps.objectManager.createObject", [guid.ToString(), functionName, .. args]);

        return jsObjectRef;
    }

    public async static Task<Dictionary<Guid, JsObjectRef>> CreateMultipleAsync(
        IJSRuntime jsRuntime,
        string functionName,
        Dictionary<Guid, object> dictArgs)
    {
        var jsObjectRefs = dictArgs.ToDictionary(e => e.Key, e => new JsObjectRef(jsRuntime, e.Key));

        _ = await jsRuntime.MyInvokeAsync<object>(
            "blazorGoogleMaps.objectManager.createMultipleObject",
            [dictArgs.Select(e => e.Key.ToString()).ToList(), functionName, .. dictArgs.Values]
        );

        return jsObjectRefs;
    }

    public virtual void Dispose() => _ = DisposeAsync();

    public ValueTask<object> DisposeAsync() => JSRuntime.InvokeAsync<object>("blazorGoogleMaps.objectManager.disposeObject", Guid.ToString());

    public ValueTask<object> DisposeMultipleAsync(List<Guid> guids)
        => JSRuntime.InvokeAsync<object>("blazorGoogleMaps.objectManager.disposeMultipleObjects", guids.Select(e => e.ToString()).ToList());

    public async Task InvokeAsync(string functionName, params object?[] args)
        => await JSRuntime.MyInvokeAsync("blazorGoogleMaps.objectManager.invoke", [Guid.ToString(), functionName, .. args]);

    public Task InvokeMultipleAsync(string functionName, Dictionary<Guid, object> dictArgs)
        => JSRuntime.MyInvokeAsync("blazorGoogleMaps.objectManager.invokeMultiple", [dictArgs.Select(e => e.Key.ToString()).ToList(), functionName, .. dictArgs.Values]);

    public Task AddMultipleListenersAsync(string eventName, Dictionary<Guid, object> dictArgs)
        => JSRuntime.MyAddListenerAsync("blazorGoogleMaps.objectManager.addMultipleListeners", [dictArgs.Select(e => e.Key.ToString()).ToList(), eventName, .. dictArgs.Values]);

    public Task InvokePropertyAsync(string functionName, params object?[] args)
        => JSRuntime.MyInvokeAsync("blazorGoogleMaps.objectManager.invokeProperty", [Guid.ToString(), functionName, .. args]);

    public Task<T> InvokePropertyAsync<T>(string functionName, params object?[] args) 
        => JSRuntime.MyInvokeAsync<T>("blazorGoogleMaps.objectManager.invokeProperty", [Guid.ToString(), functionName, .. args]);

    public Task<T> InvokeAsync<T>(string functionName, params object?[] args)
    {
        return JSRuntime.MyInvokeAsync<T>(
            "blazorGoogleMaps.objectManager.invoke",
            [Guid.ToString(), functionName, .. args]
        );
    }

    public Task<Dictionary<string, T>> InvokeMultipleAsync<T>(string functionName, Dictionary<Guid, object> dictArgs)
    {
        return JSRuntime.MyInvokeAsync<Dictionary<string, T>>(
            "blazorGoogleMaps.objectManager.invokeMultiple",
            [dictArgs.Select(e => e.Key.ToString()).ToList(), functionName, .. dictArgs.Values]
        );
    }

    /// <summary>
    /// Use when returned result will be one of defined types
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <param name="functionName"></param>
    /// <param name="args"></param>
    /// <returns>Discriminated union of specified types</returns>
    public async Task<OneOf<T, U>> InvokeAsync<T, U>(string functionName, params object[] args)
    {
        OneOf<T, U> result = await JSRuntime.MyInvokeAsync<T, U>("blazorGoogleMaps.objectManager.invoke", [Guid.ToString(), functionName, .. args]);

        return result;
    }

    /// <summary>
    /// Use when returned result will be one of defined types
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="functionName"></param>
    /// <param name="args"></param>
    /// <returns>Discriminated union of specified types</returns>
    public Task<OneOf<T, U, V>> InvokeAsync<T, U, V>(string functionName, params object[] args) => JSRuntime.MyInvokeAsync<T, U, V>("blazorGoogleMaps.objectManager.invoke", [Guid.ToString(), functionName, .. args]);

    public async Task<JsObjectRef> InvokeWithReturnedObjectRefAsync(string functionName, params object[] args)
    {
        string? guid = await JSRuntime.MyInvokeAsync<string>(
            "blazorGoogleMaps.objectManager.invokeWithReturnedObjectRef",
            [Guid.ToString(), functionName, .. args]
        );

        return new JsObjectRef(JSRuntime, new Guid(guid));
    }

    //public async Task<List<JsObjectRef>> InvokeMultipleWithReturnedObjectRefAsync(string functionName, string eventname, Dictionary<Guid, object> dictArgs)
    //{
    //    List<string> guids = await JSRuntime.MyInvokeAsync<List<string>>(
    //        "blazorGoogleMaps.objectManager.invokeMultipleWithReturnedObjectRef",
    //        new object[] { dictArgs.Select(e => e.Key.ToString()).ToList(), functionName, eventname }
    //            .Concat(dictArgs.Values).ToArray()
    //    );

    //    return guids.Select(e => new JsObjectRef(JSRuntime, new Guid(e))).ToList();
    //}

    public Task<T> GetValue<T>(string propertyName)
    {
        return JSRuntime.MyInvokeAsync<T>(
            "blazorGoogleMaps.objectManager.readObjectPropertyValue",
            Guid.ToString(),
            propertyName);
    }

    public async Task<JsObjectRef> GetObjectReference(string propertyName)
    {
        string? guid = await JSRuntime.MyInvokeAsync<string>(
            "blazorGoogleMaps.objectManager.readObjectPropertyValueWithReturnedObjectRef",
            Guid.ToString(),
            propertyName);

        return new JsObjectRef(JSRuntime, new Guid(guid));
    }

    public Task<T?> GetMappedValue<T>(string propertyName, params string[] mappedNames)
    {
        return JSRuntime.MyInvokeAsync<T>(
            "blazorGoogleMaps.objectManager.readObjectPropertyValueAndMapToArray",
            Guid.ToString(),
            propertyName, mappedNames);
    }

    public override bool Equals(object obj) => obj is JsObjectRef other && other.Guid == Guid;

    public override int GetHashCode() => Guid.GetHashCode();
}
