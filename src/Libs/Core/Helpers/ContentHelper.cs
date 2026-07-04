namespace Seedysoft.Libs.Core.Helpers;

public static class ContentHelper
{
    //public static string ContentPath => $"_content/{typeof(ContentHelper).Assembly.GetName().Name}";
    public static string ContentPath(Type type) => $"_content/{type.Assembly.GetName().Name}";

    public static string ReplaceSpaceWithPlus(this string text) => string.IsNullOrWhiteSpace(text) ? text : text.Replace(' ', '+');
}
