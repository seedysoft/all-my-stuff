namespace Seedysoft.Libs.TelegramBot.Settings;

public record TelegramUserBase
{
    public string Id
    {
        get;
        init => field = Cryptography.Crypto.DecryptText(value, Core.Helpers.EnvironmentHelper.GetMasterKey());
    } = default!;
    public string Username
    {
        get;
        init => field = Cryptography.Crypto.DecryptText(value, Core.Helpers.EnvironmentHelper.GetMasterKey());
    } = default!;
}
