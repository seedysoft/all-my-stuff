namespace Seedysoft.Libs.TelegramBot.Settings;

public record class TelegramUser
{
    public required TelegramBotUser BotProd { get; init; }
    public required TelegramBotUser BotTest { get; init; }

    public required TelegramKnowUser UserTest { get; init; }
}
