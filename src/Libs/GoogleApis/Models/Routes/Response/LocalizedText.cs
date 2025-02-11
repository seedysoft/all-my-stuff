﻿namespace Seedysoft.Libs.GoogleApis.Models.Routes.Response;

/// <summary>
/// Localized variant of a text in a particular language.
/// </summary>
public record LocalizedText
{
    /// <summary>
    /// Localized string in the language corresponding to languageCode below.
    /// </summary>
    [J("text")]
    public required string Text { get; init; }

    /// <summary>
    /// The text's BCP-47 language code, such as "en-US" or "sr-Latn".
    /// For more information, see http://www.unicode.org/reports/tr35/#Unicode_locale_identifier.
    /// </summary>
    [J("languageCode"), I(Condition = C.WhenWritingNull)]
    public string? LanguageCode { get; init; }
}
