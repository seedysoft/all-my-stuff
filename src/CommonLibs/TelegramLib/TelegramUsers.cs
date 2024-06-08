using Seedysoft.UtilsLib.Helpers;

namespace Seedysoft.TelegramLib;

public record TelegramUserBase
{
    private string id = default!;
    private string username = default!;

    public string Id
    {
        get => id;
        init => id = CryptoLib.Crypto.DecryptText(value, EnvironmentHelper.GetMasterKey());
    }
    public string Username
    {
        get => username;
        init => username = CryptoLib.Crypto.DecryptText(value, EnvironmentHelper.GetMasterKey());
    }
}

public record TelegramBotUser : TelegramUserBase
{
    private string token = default!;
    public string Token
    {
        get => token;
        init => token = CryptoLib.Crypto.DecryptText(value, EnvironmentHelper.GetMasterKey());
    }

    public Telegram.Bot.Types.User? SenderUser { get; protected set; }

    public void SetMe(Telegram.Bot.Types.User user) => SenderUser = user;
}

public record TelegramKnowUser : TelegramUserBase { }

public record TelegramUser
{
    public required TelegramBotUser BotProd { get; init; }
    public required TelegramBotUser BotTest { get; init; }

    public required TelegramKnowUser UserTest { get; init; }
}
