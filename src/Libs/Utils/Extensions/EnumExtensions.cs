using System.Reflection;

namespace Seedysoft.Libs.Utils.Extensions;

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

    //public static T? FromEnumValue<T>(string enumMemberValue) where T : Enum
    //{
    //    MemberInfo? memberInfo = typeof(T)
    //        .GetTypeInfo()
    //        .DeclaredMembers
    //        .FirstOrDefault(x => x.GetCustomAttribute<System.Runtime.Serialization.EnumMemberAttribute>()?.Value == enumMemberValue);

    //    return memberInfo == null ? default : (T)typeof(T).GetField(memberInfo.Name).GetValue(null);
    //}
    public static T? ToEnum<T>(string str)
    {
        Type enumType = typeof(T);
        foreach (string name in Enum.GetNames(enumType))
        {
            System.Runtime.Serialization.EnumMemberAttribute enumMemberAttribute = 
                (enumType.GetField(name).GetCustomAttributes(typeof(System.Runtime.Serialization.EnumMemberAttribute), true) as System.Runtime.Serialization.EnumMemberAttribute[]).Single();
            
            if (enumMemberAttribute.Value == str)
                return (T)Enum.Parse(enumType, name);

            if (string.Equals(name, str, StringComparison.InvariantCultureIgnoreCase))
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
            Attribute.GetCustomAttribute(fieldInfo, typeof(System.Runtime.Serialization.EnumMemberAttribute)) is System.Runtime.Serialization.EnumMemberAttribute EnumMemberAttrb)
        {
            member = EnumMemberAttrb.Value;
            return true;
        }

        member = null;
        return false;
    }

    public static string ToCssString(this Core.Enums.CssUnit unit) =>
        unit switch
        {
            Core.Enums.CssUnit.Em => "em",
            Core.Enums.CssUnit.Percentage => "%",
            Core.Enums.CssUnit.Pt => "pt",
            Core.Enums.CssUnit.Px => "px",
            Core.Enums.CssUnit.Rem => "rem",
            Core.Enums.CssUnit.Vh => "vh",
            Core.Enums.CssUnit.VMax => "vmax",
            Core.Enums.CssUnit.VMin => "vmin",
            Core.Enums.CssUnit.Vw => "vw",
            _ => string.Empty
        };
}
