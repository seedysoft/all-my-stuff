namespace Seedysoft.Libs.TelegramBot.Settings;

public record class TelegramBotSettings : BackgroundServices.ScheduleConfig
{
    public required Users Users { get; init; }

    public TelegramBotUser CurrentBot => System.Diagnostics.Debugger.IsAttached ? Users.BotTest : Users.BotProd;
}

public record class Users
{
    public required TelegramBotUser BotProd { get; init; }
    public required TelegramBotUser BotTest { get; init; }
    public required TelegramKnowUser UserTest { get; init; }
}

public record class TelegramBotUser : TelegramUserBase
{
    public string Token
    {
        get;
        init => field = Cryptography.Crypto.Decrypt(Core.Helpers.EnvironmentHelper.GetMasterKey(), value);
    } = default!;

    public Telegram.Bot.Types.User? SenderUser { get; protected set; }

    public void SetMe(Telegram.Bot.Types.User user) => SenderUser = user;
}

public record class TelegramKnowUser : TelegramUserBase { }

public record class TelegramUser
{
    public required TelegramBotUser BotProd { get; init; }
    public required TelegramBotUser BotTest { get; init; }

    public required TelegramKnowUser UserTest { get; init; }
}

public abstract record class TelegramUserBase
{
    public string Id
    {
        get;
        init => field = Cryptography.Crypto.Decrypt(Core.Helpers.EnvironmentHelper.GetMasterKey(), value);
    } = default!;
    public string Username
    {
        get;
        init => field = Cryptography.Crypto.Decrypt(Core.Helpers.EnvironmentHelper.GetMasterKey(), value);
    } = default!;
}
