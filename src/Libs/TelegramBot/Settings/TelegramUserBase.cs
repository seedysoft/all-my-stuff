namespace Seedysoft.Libs.TelegramBot.Settings;

public record TelegramUserBase
{
    private string id = default!;
    private string username = default!;

    public string Id
    {
        get => id;
        init => id = Cryptography.Crypto.DecryptText(value, Utils.Helpers.EnvironmentHelper.GetMasterKey());
    }
    public string Username
    {
        get => username;
        init => username = Cryptography.Crypto.DecryptText(value, Utils.Helpers.EnvironmentHelper.GetMasterKey());
    }
}
