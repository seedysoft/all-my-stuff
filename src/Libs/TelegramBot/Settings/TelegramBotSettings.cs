namespace Seedysoft.Libs.TelegramBot.Settings;

public record TelegramBotSettings : BackgroundServices.ScheduleConfig
{
    public required TelegramBot BotProd { get; init; }
    public required TelegramBot BotTest { get; init; }

    public required TelegramKnowUser KnownUserForTest { get; init; }

    public TelegramBot CurrentBot
    {
        get
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
                return BotTest;
            }
            else
            {
                return BotProd;
            }
        }
    }
}

public record TelegramBot : TelegramAccountBase
{
    public required string Token
    {
        get;
        init => field = Cryptography.Crypto.DecryptText(value, Core.Helpers.EnvironmentHelper.GetMasterKey());
    }
    public string FullToken => $"{Id}:{Token}";

    public Telegram.Bot.Types.User? SenderUser { get; protected set; }

    public void SetMe(Telegram.Bot.Types.User user) => SenderUser = user;
}

public record TelegramKnowUser : TelegramAccountBase { }

public abstract record TelegramAccountBase
{
    public required string Id
    {
        get;
        init => field = Cryptography.Crypto.DecryptText(value, Core.Helpers.EnvironmentHelper.GetMasterKey());
    }
    public long IdAsLong => long.Parse(Id);

    public required string Username
    {
        get;
        init => field = Cryptography.Crypto.DecryptText(value, Core.Helpers.EnvironmentHelper.GetMasterKey());
    }
}
