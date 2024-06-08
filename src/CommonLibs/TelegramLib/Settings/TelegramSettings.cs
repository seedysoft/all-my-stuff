namespace Seedysoft.TelegramLib.Settings;

public record TelegramSettings : CronBackgroundServiceLib.ScheduleConfig
{
    public required Users Users { get; init; }

    public TelegramBotUser CurrentBot => System.Diagnostics.Debugger.IsAttached ? Users.BotTest : Users.BotProd;
}

public record Users
{
    public required TelegramBotUser BotProd { get; init; }
    public required TelegramBotUser BotTest { get; init; }
    public required TelegramKnowUser UserTest { get; init; }
}
