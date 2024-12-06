namespace Seedysoft.Libs.Utils.Extensions;

public static class EnumExtensions
{
    public static string GetEnumDescription(this Enum enumValue) =>
        enumValue.TryGetEnumDescription(out string? description) ? description! : enumValue.ToString();

    public static bool TryGetEnumDescription(this Enum enumValue, out string? description)
    {
        System.Reflection.FieldInfo? fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

        if (fieldInfo != null &&
            Attribute.GetCustomAttribute(fieldInfo, typeof(System.ComponentModel.DescriptionAttribute)) is System.ComponentModel.DescriptionAttribute DescriptionAttrb)
        {
            description = DescriptionAttrb.Description;
            return true;
        }

        description = null;
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
