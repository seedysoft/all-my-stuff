namespace Seedysoft.Libs.TelegramBot.Settings;

public record class TelegramBotSettings : BackgroundServices.ScheduleConfig
{
    public required TelegramUsers Users { get; init; }

    public TelegramBotUser CurrentBot => System.Diagnostics.Debugger.IsAttached ? Users.BotTest : Users.BotProd;
}

public record class TelegramUsers
{
    public required TelegramBotUser BotProd { get; init; }
    public required TelegramBotUser BotTest { get; init; }
    public required TelegramKnowUser UserTest { get; init; }
}

public record class TelegramBotUser : TelegramUserBase
{
    public required string Token
    {
        get;
        init => field = Core.Helpers.EnvironmentHelper.Decrypt(value);
    }
    public string FullToken => $"{Id}:{Token}";

    public Telegram.Bot.Types.User? SenderUser { get; protected set; }

    public void SetMe(Telegram.Bot.Types.User user) => SenderUser = user;
}

public record class TelegramKnowUser : TelegramUserBase { }

public abstract record class TelegramUserBase
{
    public required string Id
    {
        get;
        init => field = Core.Helpers.EnvironmentHelper.Decrypt(value);
    }
    public long IdAsLong => long.Parse(Id);

    public required string Username
    {
        get;
        init => field = Core.Helpers.EnvironmentHelper.Decrypt(value);
    }
}
