using System.Reflection;
using System.Runtime.Serialization;

namespace Seedysoft.Libs.Core.Extensions;

public static class EnumExtensions
{
    public static string GetEnumDescription(this Enum enumValue) =>
        enumValue.TryGetEnumDescription(out string? description) ? description! : enumValue.ToString();
    public static bool TryGetEnumDescription(this Enum enumValue, out string? description)
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

    public static T? ToEnum<T>(string str)
    {
        Type enumType = typeof(T);
        foreach (string name in Enum.GetNames(enumType))
        {
            EnumMemberAttribute enumMemberAttribute =
                (enumType.GetField(name)!.GetCustomAttributes(typeof(EnumMemberAttribute), true) as EnumMemberAttribute[])!.Single();

            if (enumMemberAttribute.Value == str || string.Equals(name, str, StringComparison.InvariantCultureIgnoreCase))
                return (T)Enum.Parse(enumType, name);
        }

        //throw exception or whatever handling you want
        return default;
    }
    public static string GetEnumMember(this Enum enumValue) =>
        enumValue.TryGetEnumMember(out string? member) ? member! : enumValue.ToString();
    public static bool TryGetEnumMember(this Enum enumValue, out string? member)
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

    public static string ToCssString(this Enums.CssUnit unit) =>
        unit switch
        {
            Enums.CssUnit.Em => "em",
            Enums.CssUnit.Percentage => "%",
            Enums.CssUnit.Pt => "pt",
            Enums.CssUnit.Px => "px",
            Enums.CssUnit.Rem => "rem",
            Enums.CssUnit.Vh => "vh",
            Enums.CssUnit.VMax => "vmax",
            Enums.CssUnit.VMin => "vmin",
            Enums.CssUnit.Vw => "vw",
            _ => string.Empty
        };
}
