using GoogleMapsLibrary.Extensions;
using GoogleMapsLibrary.Maps;
using Microsoft.JSInterop;

namespace GoogleMapsLibrary;

public class GmpJsInterop(IJSRuntime jsRuntime) : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask =
        new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/GoogleMapsJavascriptApi/gmpJsInterop.js").AsTask());

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

    public async ValueTask InitMap(MapOptions mapOptions) => await InvokeVoidAsync("InitMap", mapOptions);
}
