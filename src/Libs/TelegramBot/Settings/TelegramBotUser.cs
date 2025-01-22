namespace Seedysoft.Libs.TelegramBot.Settings;

public record TelegramBotUser : TelegramUserBase
{
    private string token = default!;
    public string Token
    {
        get => token;
        init => token = Cryptography.Crypto.DecryptText(value, Core.Helpers.EnvironmentHelper.GetMasterKey());
    }

    public Telegram.Bot.Types.User? SenderUser { get; protected set; }

    public void SetMe(Telegram.Bot.Types.User user) => SenderUser = user;
}
