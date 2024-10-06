using GoogleMapsLibrary.Interfaces;
using Microsoft.JSInterop;
using System.Text.Json;

namespace GoogleMapsLibrary;

public class JsCallableAction
{
    private readonly Delegate _delegate;
    private readonly Type[] _argumentTypes;
    private readonly IJSRuntime _jsRuntime;

    public JsCallableAction(IJSRuntime jsRuntime, Delegate @delegate, params Type[] argumentTypes)
    {
        _jsRuntime = jsRuntime;
        _delegate = @delegate;
        _argumentTypes = argumentTypes;
    }

    [JSInvokable]
    public void Invoke(string args, string guid)
    {
        if (string.IsNullOrWhiteSpace(args) || _argumentTypes.Length == 0)
        {
            _ = _delegate.DynamicInvoke();
            return;
        }

        JsonElement.ArrayEnumerator jArray = JsonDocument.Parse(args)
            .RootElement
            .EnumerateArray();

        object?[] arguments = _argumentTypes.Zip(jArray, (type, jToken) => new { jToken, type })
            .Select(x =>
            {
                object? obj =Serialization. Helper.DeSerializeObject(x.jToken, x.type);
                if (obj is IActionArgument actionArg)
                    actionArg.JsObjectRef = new JsObjectRef(_jsRuntime, new Guid(guid));

                return obj;
            })
            .ToArray();

        _ = _delegate.DynamicInvoke(arguments);
    }
}
