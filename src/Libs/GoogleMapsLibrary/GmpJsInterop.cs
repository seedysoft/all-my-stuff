using GoogleMapsLibrary.Maps;
using Microsoft.JSInterop;

namespace GoogleMapsLibrary;

public class GmpJsInterop(IJSRuntime jsRuntime) : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask =
        new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/GoogleMapsJavascriptApi/js/gmpJsInterop.js").AsTask());

    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            IJSObjectReference module = await moduleTask.Value;
            await module.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }

    public async ValueTask InvokeVoidAsync(string jsFunctionName, params object?[]? args)
        => await (await moduleTask.Value).InvokeVoidAsync($"{jsFunctionName}", args);

    public async ValueTask<T> InvokeAsync<T>(string jsFunctionName, params object?[]? args)
        => await (await moduleTask.Value).InvokeAsync<T>($"{jsFunctionName}", args);

    //public async ValueTask<GmpJsInterop> InvokeWithReturnedObjectRefAsync(string jsFunctionName, params object[] args)
    //{
    //    string? guid = await jsRuntime.MyInvokeAsync<string>("invokeWithReturnedObjectRef", [/*_guid.ToString(),*/ jsFunctionName, .. args]);

    //    return new GmpJsInterop(jsRuntime/*, new Guid(guid)*/);
    //}

    //public async Task<GmpJsInterop> GetObjectReference(string propertyName)
    //{
    //    var guid = await (await moduleTask.Value).MyInvokeAsync<string>(
    //        "blazorGoogleMaps.objectManager.readObjectPropertyValueWithReturnedObjectRef",
    //        _guid.ToString(),
    //        propertyName);

    //    return new GmpJsInterop(jsRuntime/*, new Guid(guid)*/);
    //}

    public async ValueTask InitMap(MapOptions mapOptions) => await InvokeVoidAsync("initMap", mapOptions);

    //public static Task<GmpJsInterop> CreateAsync(
    //    IJSRuntime jsRuntime,
    //    string constructorFunctionName,
    //    params object?[] args)
    //    => CreateAsync(jsRuntime, Guid.NewGuid(), constructorFunctionName, args);
    //public async static Task<GmpJsInterop> CreateAsync(
    //    IJSRuntime jsRuntime,
    //    Guid guid,
    //    string functionName,
    //    params object?[] args)
    //{
    //    var jsObjectRef = new GmpJsInterop(jsRuntime/*, guid*/);

    //    _ = await jsRuntime.MyInvokeAsync<object>("blazorGoogleMaps.objectManager.createObject", [guid.ToString(), functionName, .. args]);

    //    return jsObjectRef;
    //}
}
