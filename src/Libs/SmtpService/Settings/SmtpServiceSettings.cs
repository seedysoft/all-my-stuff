namespace Seedysoft.Libs.SmtpService.Settings;

public record SmtpServiceSettings
{
    public required string Host { get; init; }

    public int Port { get; init; }

    public required string Username { get; init; }

    public string Password
    {
        get;
        // init => field = Cryptography.Crypto.Decrypt(Core.Helpers.EnvironmentHelper.GetMasterKey(), value);
        init => field = Cryptography.Crypto.DecryptText(value, Core.Helpers.EnvironmentHelper.GetMasterKey());
    } = default!;
}
