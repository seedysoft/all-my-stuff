using GoogleMapsLibrary.Interfaces;
using Microsoft.JSInterop;
using System.Text.Json;

namespace GoogleMapsLibrary;

public class JsCallableAction(IJSRuntime jsRuntime, Delegate @delegate, params Type[] argumentTypes)
{
    [JSInvokable]
    public void Invoke(string args, string guid)
    {
        if (string.IsNullOrWhiteSpace(args) || argumentTypes.Length == 0)
        {
            _ = @delegate.DynamicInvoke();
            return;
        }

        JsonElement.ArrayEnumerator jArray = JsonDocument.Parse(args)
            .RootElement
            .EnumerateArray();

        object?[] arguments = argumentTypes.Zip(jArray, (type, jToken) => new { jToken, type })
            .Select(x =>
            {
                object? obj = Serialization.Helper.DeSerializeObject(x.jToken, x.type);
                if (obj is IActionArgument actionArg)
                    actionArg.GmpJsInterop = new GmpJsInterop(jsRuntime/*, new Guid(guid)*/);

                return obj;
            })
            .ToArray();

        _ = @delegate.DynamicInvoke(arguments);
    }
}
