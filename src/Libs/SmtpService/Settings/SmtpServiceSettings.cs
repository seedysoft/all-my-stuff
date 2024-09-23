namespace Seedysoft.Libs.SmtpService.Settings;

public record class SmtpServiceSettings
{
    public required string Host { get; init; }

    public int Port { get; init; }

    public required string Username { get; init; }

    private string password = default!;
    public string Password
    {
        get => password;
        init => password = Cryptography.Crypto.DecryptText(value, Utils.Helpers.EnvironmentHelper.GetMasterKey());
    }
}
