namespace Seedysoft.Libs.Core.Constants;

/// <summary>
/// Contains constant string values used throughout the Seedysoft application.
/// </summary>
public static class Strings
{
    /// <summary>
    /// A constant prefix string used to indicate a failure message.
    /// </summary>
    public const string FailText = "Fail: ";

    /// <summary>
    /// The time zone identifier for Romance Standard Time (typically Europe/Madrid or similar European timezone).
    /// Used for time zone conversions and localization purposes.
    /// </summary>
    public const string RomanceStandardTimeZoneId = "Romance Standard Time";

    /// <summary>
    /// A localized message prefix indicating that a subscription was received via Telegram.
    /// Value: "Recibido a través de Telegram el " (Spanish: "Received via Telegram on ")
    /// Typically used as a prefix for subscription notification messages.
    /// </summary>
    public const string TextForNewSubscription = "Recibido a través de Telegram el ";
}
