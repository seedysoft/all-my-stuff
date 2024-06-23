namespace Seedysoft.Libs.SmtpService.Settings;

public record SmtpServiceSettings
{
    public required string Host { get; init; }

    public int Port { get; init; }

    public required string Username { get; init; }

    private string password = default!;
    public string Password
    {
        get => password;
        init => password = Crypto.Crypto.DecryptText(value, Utils.Helpers.EnvironmentHelper.GetMasterKey());
    }
}
