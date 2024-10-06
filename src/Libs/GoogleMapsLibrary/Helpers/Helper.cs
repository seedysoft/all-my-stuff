using GoogleMapsLibrary.Interfaces;
using GoogleMapsLibrary.Maps;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OneOf;
using System.Text.Json;

namespace GoogleMapsLibrary.Helpers;

public static class Helper
{
    internal static Task MyInvokeAsync(
        this IJSRuntime jsRuntime,
        string identifier,
        params object?[] args) => jsRuntime.MyInvokeAsync<object>(identifier, args);

    //internal static T? ToNullableEnum<T>(string? str)
    //    where T : struct
    //{
    //    var enumType = typeof(T);

    //    if (int.TryParse(str, out var enumIntValue))
    //    {
    //        return (T)Enum.Parse(enumType, enumIntValue.ToString());
    //    }

    //    if (str == "null")
    //    {
    //        return null;
    //    }

    //    foreach (var name in Enum.GetNames(enumType))
    //    {
    //        var enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();
    //        if (enumMemberAttribute.Value == str)
    //        {
    //            return (T)Enum.Parse(enumType, name);
    //        }
    //    }

    //    //throw exception or whatever handling you want
    //    return default;
    //}

    private static IEnumerable<object> MakeArgJsFriendly(IJSRuntime jsRuntime, IEnumerable<object?> args)
    {
        IEnumerable<object?> jsFriendlyArgs = args
            .Select(arg =>
            {
                if (arg == null)
                    return arg;

                if (arg is IOneOf oneof)
                    arg = oneof.Value;

                switch (arg)
                {
                    case Enum:
                        return GetEnumValue(arg);

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
                        if (argType.IsGenericType && (argType.GetGenericTypeDefinition() == typeof(Action<>)))
                        {
                            Type[] genericArguments = argType.GetGenericArguments();

                            //Debug.WriteLine($"Generic args : {genericArguments.Count()}");

                            return DotNetObjectReference.Create(new JsCallableAction(jsRuntime, (Delegate)arg, genericArguments));
                        }

                        switch (arg)
                        {
                            case JsCallableAction _:
                                return DotNetObjectReference.Create(arg);

                            case IJsObjectRef jsObjectRef:
                                //Debug.WriteLine("Serialize IJsObjectRef");

                                Guid guid = jsObjectRef.Guid;
                                return Serialization.Helper.SerializeObject(new JsObjectRef1(guid));

                            default:
                                return Serialization.Helper.SerializeObject(arg);
                        }
                }
            });

        return jsFriendlyArgs;
    }

    private static string? GetEnumValue(object? enumItem)
    {
        //what happens if enumItem is null.
        //Shouldnt we take 0 value of enum
        //Also is it even possible to have null enumItem
        //So far looks like only MapLegend Add controll reach here
        if (enumItem == null)
            return null;

        if (enumItem is not Enum enumItem2)
            return enumItem.ToString();

        System.Reflection.MemberInfo[] memberInfo = enumItem2.GetType().GetMember(enumItem2.ToString());

        return memberInfo.Length == 0
            ? null
            : (memberInfo[0].GetCustomAttributes(false).OfType<EnumMemberAttribute>().FirstOrDefault()?.Value);
    }

    internal static async Task<TRes?> MyInvokeAsync<TRes>(
        this IJSRuntime jsRuntime,
        string identifier,
        params object?[] args)
    {
        IEnumerable<object> jsFriendlyArgs = MakeArgJsFriendly(jsRuntime, args);

        if (typeof(IJsObjectRef).IsAssignableFrom(typeof(TRes)))
        {
            string? guid = await jsRuntime.InvokeAsync<string?>(identifier, jsFriendlyArgs);

            return guid == null ? default : (TRes)JsObjectRefInstances.GetInstance(guid);
        }

        if (typeof(IOneOf).IsAssignableFrom(typeof(TRes)))
        {
            string resultObject = await jsRuntime.InvokeAsync<string>(identifier, jsFriendlyArgs);
            object? result = null;

            if (resultObject is string someText)
            {
                try
                {
                    var jo = JsonDocument.Parse(someText);
                    string? typeToken = jo.RootElement.GetProperty("dotnetTypeName").GetString();
                    result = typeToken == null ? (object)someText : Serialization.Helper.DeSerializeObject<TRes>(typeToken);
                }
                catch
                {
                    result = someText;
                }
            }

            return (TRes?)result;
        }
        else
        {
            return await jsRuntime.InvokeAsync<TRes>(identifier, jsFriendlyArgs);
        }
    }

    internal static async Task<object> MyAddListenerAsync(this IJSRuntime jsRuntime, string identifier, params object[] args)
    {
        IEnumerable<object> jsFriendlyArgs = MakeArgJsFriendly(jsRuntime, args);

        return await jsRuntime.InvokeAsync<object>(identifier, jsFriendlyArgs);
    }

    /// <summary>
    /// For use when returned result will be one of multiple type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <param name="jsRuntime"></param>
    /// <param name="identifier"></param>
    /// <param name="args"></param>
    /// <returns>Discriminated union of specified types</returns>
    internal static async Task<OneOf<T, U>> MyInvokeAsync<T, U>(
        this IJSRuntime jsRuntime,
        string identifier,
        params object[] args)
    {
        object? resultObject = await jsRuntime.MyInvokeAsync<object>(identifier, args);
        object? result = null;

        if (resultObject is JsonElement jsonElement)
        {
            string? json;
            if (jsonElement.ValueKind == JsonValueKind.Number)
            {
                json = jsonElement.GetRawText();
            }
            else if (jsonElement.ValueKind == JsonValueKind.String)
            {
                json = jsonElement.GetString();
                //Not sure if this is ok
                //Basically fails for marker GetLabel if skip this
                if (typeof(T) == typeof(string))
                {
                    result = json ?? string.Empty;
                    return (T)result;
                }

                if (typeof(U) == typeof(string))
                {
                    result = json ?? string.Empty;
                    return (T)result;
                }
            }
            else
            {
                json = jsonElement.GetString();
            }

            Dictionary<string, object>? propArray = Serialization.Helper.DeSerializeObject<Dictionary<string, object>>(json);
            if (propArray?.TryGetValue("dotnetTypeName", out object? typeName) ?? false)
            {
                System.Reflection.Assembly asm = typeof(Map).Assembly;
                string? typeNameString = typeName.ToString();
                Type? type = asm.GetType(typeNameString);
                result = Serialization.Helper.DeSerializeObject(json, type);
            }
        }

        return result switch
        {
            T t => (OneOf<T, U>)t,
            U u => (OneOf<T, U>)u,
            _ => default,
        };
    }

    /// <summary>
    /// For use when returned result will be one of multiple type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="jsRuntime"></param>
    /// <param name="identifier"></param>
    /// <param name="args"></param>
    /// <returns>Discriminated union of specified types</returns>
    internal static async Task<OneOf<T, U, V>> MyInvokeAsync<T, U, V>(
        this IJSRuntime jsRuntime,
        string identifier,
        params object[] args)
    {
        object? resultObject = await jsRuntime.MyInvokeAsync<object>(identifier, args);
        object? result = null;

        if (resultObject is JsonElement jsonElement)
        {
            string? json = jsonElement.GetString();
            Dictionary<string, object>? propArray = Serialization.Helper.DeSerializeObject<Dictionary<string, object>>(json);
            if (propArray?.TryGetValue("dotnetTypeName", out object? typeName) ?? false)
            {
                System.Reflection.Assembly asm = typeof(Map).Assembly;
                string? typeNameString = typeName.ToString();
                Type? type = asm.GetType(typeNameString);
                result = Serialization.Helper.DeSerializeObject(json, type);
            }
        }

        return result switch
        {
            T t => (OneOf<T, U, V>)t,
            U u => (OneOf<T, U, V>)u,
            V v => (OneOf<T, U, V>)v,
            _ => default,
        };
    }

    internal static T? ToEnum<T>(string str)
    {
        Type enumType = typeof(T);
        foreach (string name in Enum.GetNames(enumType))
        {
            EnumMemberAttribute enumMemberAttribute = 
                (enumType.GetField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true) as EnumMemberAttribute[]).Single();
            if (enumMemberAttribute.Value == str)
                return (T)Enum.Parse(enumType, name);

            if (string.Equals(name, str, StringComparison.InvariantCultureIgnoreCase))
                return (T)Enum.Parse(enumType, name);
        }

        //throw exception or whatever handling you want
        return default;
    }
}
