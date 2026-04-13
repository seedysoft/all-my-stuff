namespace Seedysoft.Libs.TelegramBot.Settings;

public record TelegramBotUser : TelegramUserBase
{
    public string Token
    {
        get;
        init => field = Cryptography.Crypto.DecryptText(value, Core.Helpers.EnvironmentHelper.GetMasterKey());
    } = default!;

    public Telegram.Bot.Types.User? SenderUser { get; protected set; }

    public void SetMe(Telegram.Bot.Types.User user) => SenderUser = user;
}
