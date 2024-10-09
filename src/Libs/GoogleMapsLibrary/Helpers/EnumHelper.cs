namespace GoogleMapsLibrary.Helpers;

internal static class EnumHelper
{
    public static string? GetEnumValue(object? enumItem)
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

    public static T? ToEnum<T>(string str)
    {
        Type enumType = typeof(T);
        foreach (string name in Enum.GetNames(enumType))
        {
            EnumMemberAttribute enumMemberAttribute =
                enumType.GetField(name)!.GetCustomAttributes(typeof(EnumMemberAttribute), true).OfType<EnumMemberAttribute>().Single();
            if (enumMemberAttribute.Value == str)
                return (T)Enum.Parse(enumType, name);

            if (string.Equals(name, str, StringComparison.InvariantCultureIgnoreCase))
                return (T)Enum.Parse(enumType, name);
        }

        //throw exception or whatever handling you want
        return default;
    }
}
