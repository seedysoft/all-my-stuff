using Microsoft.JSInterop;

namespace Seedysoft.Libs.MapRazorClassLibrary;

public sealed class MapService(IJSRuntime jsRuntime) : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> ModuleTask = new(() => GetJSObjectReference(jsRuntime));

    private static Task<IJSObjectReference> GetJSObjectReference(IJSRuntime jsRuntime) =>
        jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", $"./{Core.Helpers.ContentHelper.ContentPath}/js/leafletService.js?v=122").AsTask();

    public async ValueTask DisposeAsync()
    {
        if (ModuleTask.IsValueCreated)
        {
            IJSObjectReference module = await ModuleTask.Value;
            await module.DisposeAsync();
        }
    }

    internal async Task<T> InvokeAsyc<T>(string methodName, params object[] parameters)
    {
        T result = default!;

        try
        {
            IJSObjectReference module = await ModuleTask.Value;
            result = parameters == null ? await module.InvokeAsync<T>(methodName) : await module.InvokeAsync<T>(methodName, parameters);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LeafletService: {ex}");
        }

        return result;
    }

    internal async Task InvokeVoidAsync(string methodName, params object[] parameters)
    {
        try
        {
            IJSObjectReference module = await ModuleTask.Value;
            if (parameters == null)
                await module.InvokeVoidAsync(methodName);
            else
                await module.InvokeVoidAsync(methodName, parameters);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LeafletService: {ex}");
        }
    }
}
