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

    public static string ToCssString(this Core.Enums.Unit unit) =>
        unit switch
        {
            Core.Enums.Unit.Em => "em",
            Core.Enums.Unit.Percentage => "%",
            Core.Enums.Unit.Pt => "pt",
            Core.Enums.Unit.Px => "px",
            Core.Enums.Unit.Rem => "rem",
            Core.Enums.Unit.Vh => "vh",
            Core.Enums.Unit.VMax => "vmax",
            Core.Enums.Unit.VMin => "vmin",
            Core.Enums.Unit.Vw => "vw",
            _ => string.Empty
    };
}
