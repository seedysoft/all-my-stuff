using GoogleMapsLibrary.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OneOf;

namespace GoogleMapsLibrary.Extensions;

internal static class IJSRuntimeExtensions
{
    public static IEnumerable<object> MakeArgJsFriendly(this IJSRuntime jsRuntime, IEnumerable<object?> args)
    {
        IEnumerable<object?> jsFriendlyArgs = args.Select(arg =>
        {
            if (arg == null)
                return arg;

            if (arg is IOneOf oneof)
                arg = oneof.Value;

            switch (arg)
            {
                case Enum:
                    return EnumHelper.GetEnumValue(arg);

                case ElementReference _:
                case string _:
                case int _:
                case long _:
                case double _:
                case float _:
                case decimal _:
                case DateTime _:
                case bool _:
                    return arg;

                case Action action:
                    return DotNetObjectReference.Create(new JsCallableAction(jsRuntime, action));

                default:
                    Type argType = arg.GetType();
                    if (argType.IsGenericType && argType.GetGenericTypeDefinition() == typeof(Action<>))
                    {
                        Type[] genericArguments = argType.GetGenericArguments();

                        //Debug.WriteLine($"Generic args : {genericArguments.Count()}");

                        return DotNetObjectReference.Create(new JsCallableAction(jsRuntime, (Delegate)arg, genericArguments));
                    }

                    switch (arg)
                    {
                        case JsCallableAction _:
                            return DotNetObjectReference.Create(arg);

                        //case IJsObjectRef jsObjectRef:
                        //    //Debug.WriteLine("Serialize IJsObjectRef");

                        //    Guid guid = jsObjectRef.Guid;
                        //    return Serialization.Helper.SerializeObject(new JsObjectRef1(guid));

                        default:
                            return Serialization.Helper.SerializeObject(arg);
                    }
            }
        });

        return jsFriendlyArgs;
    }

    //internal static Task MyInvokeAsync(this IJSRuntime jsRuntime, string identifier, params object?[] args)
    //    => jsRuntime.MyInvokeAsync<object>(identifier, args);

    //internal static async Task<TRes?> MyInvokeAsync<TRes>(this IJSRuntime jsRuntime, string identifier, params object?[] args)
    //{
    //    IEnumerable<object> jsFriendlyArgs = MakeArgJsFriendly(jsRuntime, args);

    //    if (typeof(GmpJsInterop).IsAssignableFrom(typeof(TRes)))
    //    {
    //        string? guid = await jsRuntime.InvokeAsync<string?>(identifier, jsFriendlyArgs);

    //        return guid == null ? default : /*(TRes)JsObjectRefInstances.GetInstance(guid)*/ default;
    //    }

    //    if (typeof(IOneOf).IsAssignableFrom(typeof(TRes)))
    //    {
    //        string resultObject = await jsRuntime.InvokeAsync<string>(identifier, jsFriendlyArgs);
    //        object? result = null;

    //        if (resultObject is string someText)
    //        {
    //            try
    //            {
    //                var jo = System.Text.Json.JsonDocument.Parse(someText);
    //                string? typeToken = jo.RootElement.GetProperty("dotnetTypeName").GetString();
    //                result = typeToken == null ? (object)someText : Serialization.Helper.DeSerializeObject<TRes>(typeToken);
    //            }
    //            catch
    //            {
    //                result = someText;
    //            }
    //        }

    //        return (TRes?)result;
    //    }
    //    else
    //    {
    //        return await jsRuntime.InvokeAsync<TRes>(identifier, jsFriendlyArgs);
    //    }
    //}
}
