﻿namespace Seedysoft.UtilsLib.Extensions;

public static class EnumExtensions
{
    //public static string GetEnumDescription(this Enum enumValue)
    //{
    //    System.Reflection.FieldInfo? fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

    //    return fieldInfo != null && Attribute.GetCustomAttribute(fieldInfo, typeof(System.ComponentModel.DescriptionAttribute)) is System.ComponentModel.DescriptionAttribute DescriptionAttrb
    //        ? DescriptionAttrb.Description
    //        : throw new ArgumentException("Item not found", nameof(enumValue));
    //}

    public static bool TryGetEnumDescription(this Enum enumValue, out string? description)
    {
        System.Reflection.FieldInfo? fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

        if (fieldInfo != null && Attribute.GetCustomAttribute(fieldInfo, typeof(System.ComponentModel.DescriptionAttribute)) is System.ComponentModel.DescriptionAttribute DescriptionAttrb)
        {
            description = DescriptionAttrb.Description;
            return true;
        }

        description = null;
        return false;
    }
}
