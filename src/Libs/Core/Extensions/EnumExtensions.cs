using System.Reflection;
using System.Runtime.Serialization;

namespace Seedysoft.Libs.Core.Extensions;

public static class EnumExtensions
{
    public static string GetEnumDescription(this Enum enumValue) =>
        enumValue.TryGetEnumDescription(out string? description) ? description! : enumValue.ToString();
    private static bool TryGetEnumDescription(this Enum enumValue, out string? description)
    {
        FieldInfo? fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

        if (fieldInfo != null &&
            Attribute.GetCustomAttribute(fieldInfo, typeof(System.ComponentModel.DescriptionAttribute)) is System.ComponentModel.DescriptionAttribute DescriptionAttrb)
        {
            description = DescriptionAttrb.Description;
            return true;
        }

        description = null;
        return false;
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
    public static string GetEnumMember(this Enum enumValue) =>
        enumValue.TryGetEnumMember(out string? member) ? member! : enumValue.ToString();
    private static bool TryGetEnumMember(this Enum enumValue, out string? member)
    {
        FieldInfo? fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

        if (fieldInfo != null &&
            Attribute.GetCustomAttribute(fieldInfo, typeof(EnumMemberAttribute)) is EnumMemberAttribute EnumMemberAttrb)
        {
            member = EnumMemberAttrb.Value;
            return true;
        }

        member = null;
        return false;
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
