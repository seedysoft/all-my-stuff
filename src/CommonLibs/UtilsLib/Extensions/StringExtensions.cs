﻿namespace Seedysoft.UtilsLib.Extensions;

public static class StringExtensions
{
    private static readonly string[] HtmlStrings = new string[] { "<a", "</", "/>" };

    public static bool ContainsHtml(this string text) => HtmlStrings.Any(text.Contains);
}
