using Seedysoft.Libs.Utils.Helpers;
using Telegram.Bot.Types;

namespace Seedysoft.Libs.Telegram;

public record TelegramUserBase
{
    private string id = default!;
    private string username = default!;

    public string Id
    {
        get => id;
        init => id = Crypto.Crypto.DecryptText(value, EnvironmentHelper.GetMasterKey());
    }
    public string Username
    {
        get => username;
        init => username = Crypto.Crypto.DecryptText(value, EnvironmentHelper.GetMasterKey());
    }
}

public record TelegramBotUser : TelegramUserBase
{
    private string token = default!;
    public string Token
    {
        get => token;
        init => token = Crypto.Crypto.DecryptText(value, EnvironmentHelper.GetMasterKey());
    }

    public User? SenderUser { get; protected set; }

    public void SetMe(User user) => SenderUser = user;
}

public record TelegramKnowUser : TelegramUserBase { }

public record TelegramUser
{
    public required TelegramBotUser BotProd { get; init; }
    public required TelegramBotUser BotTest { get; init; }

    public required TelegramKnowUser UserTest { get; init; }
}
