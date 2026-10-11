using System.Reflection;
using System.Runtime.Serialization;

namespace Seedysoft.Libs.Core.Extensions;

public static class EnumExtensions
{
    public static string GetEnumDescription(this Enum enumValue)
    {
        FieldInfo? fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

        return fieldInfo == null || Attribute.GetCustomAttribute(fieldInfo, typeof(System.ComponentModel.DescriptionAttribute)) is not System.ComponentModel.DescriptionAttribute DescriptionAttrb
            ? enumValue.ToString()
            : DescriptionAttrb.Description!;
    }

    public static TEnum? ToEnum<TEnum>(string str) where TEnum : Enum
    {
        Type enumType = typeof(TEnum);
        foreach (string name in Enum.GetNames(enumType))
        {
            EnumMemberAttribute enumMemberAttribute =
                (enumType.GetField(name)!.GetCustomAttributes(typeof(EnumMemberAttribute), true) as EnumMemberAttribute[])!.Single();

            if (enumMemberAttribute.Value == str || string.Equals(name, str, StringComparison.InvariantCultureIgnoreCase))
                return (TEnum)Enum.Parse(enumType, name);
        }

        //throw exception or whatever handling you want
        return default;
    }
    //public static TEnum? ToEnum<TEnum>(int val) where TEnum : Enum
    //{
    //    Type enumType = typeof(TEnum);
    //    foreach (TEnum value in enumType.GetEnumValues())
    //    {
    //        EnumMemberAttribute enumMemberAttribute =
    //            (enumType.GetField(value.GetEnumMember())!.GetCustomAttributes(typeof(EnumMemberAttribute), true) as EnumMemberAttribute[])!.Single();

    //        if (enumMemberAttribute.Value == val || value == val)
    //            return (TEnum)Enum.Format(enumType, val,).Parse(enumType, name);
    //    }

    //    //throw exception or whatever handling you want
    //    return default;
    //}

    public static string GetEnumMember(this Enum enumValue)
    {
        FieldInfo? fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

        return fieldInfo == null || Attribute.GetCustomAttribute(fieldInfo, typeof(EnumMemberAttribute)) is not EnumMemberAttribute EnumMemberAttrb
            ? enumValue.ToString()
            : EnumMemberAttrb.Value!;
    }

    //    public static string ToCssString(this Enums.CssUnit unit) =>
    //        unit switch
    //        {
    //#pragma warning disable format
    //            Enums.CssUnit.Em            => "em",
    //            Enums.CssUnit.Percentage    => "%",
    //            Enums.CssUnit.Pt            => "pt",
    //            Enums.CssUnit.Px            => "px",
    //            Enums.CssUnit.Rem           => "rem",
    //            Enums.CssUnit.Vh            => "vh",
    //            Enums.CssUnit.VMax          => "vmax",
    //            Enums.CssUnit.VMin          => "vmin",
    //            Enums.CssUnit.Vw            => "vw",
    //            _                           => string.Empty
    //#pragma warning restore format
    //        };
}
