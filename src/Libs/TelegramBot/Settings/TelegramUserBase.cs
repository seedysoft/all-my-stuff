namespace Seedysoft.Libs.TelegramBot.Settings;

public record class TelegramUserBase
{
    private string id = default!;
    private string username = default!;

    public string Id
    {
        get => id;
        init => id = Cryptography.Crypto.DecryptText(value, Core.Helpers.EnvironmentHelper.GetMasterKey());
    }
    public string Username
    {
        get => username;
        init => username = Cryptography.Crypto.DecryptText(value, Core.Helpers.EnvironmentHelper.GetMasterKey());
    }
}
